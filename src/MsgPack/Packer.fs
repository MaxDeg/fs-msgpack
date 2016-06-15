module MsgPack.Packer

open System
open System.Text

let private packUInt (bytes : byte[]) = 
    let rec pack = function
        | 1 when Array.exists ((=) bytes.[0]) DataType.PositiveFixNum -> 
            Array.take 1 bytes
        | 1 -> [| yield DataType.UInt8; yield! Array.take 1 bytes |]
        | 2 when Array.get bytes 1 = 0uy -> pack 1
        | 2 -> [| yield DataType.UInt16; yield! Array.take 2 bytes |]
        | x when Array.get bytes (x - 1) = 0uy && Array.get bytes (x - 2) = 0uy ->
            pack (x - 2)
        | x when Array.get bytes (x - 1) = 255uy && Array.get bytes (x - 2) = 255uy ->
            pack (x - 2)
        | 4 -> [| yield DataType.UInt32; yield! Array.take 4 bytes |]
        | 8 -> [| yield DataType.UInt64; yield! Array.take 8 bytes |]
        | _ -> failwith "Cannot happen. failing packing uint"

    Array.length bytes |> pack

let private packInt (bytes : byte[]) = 
    let rec pack = function 
        | 1 when Array.exists ((=) bytes.[0]) DataType.PositiveFixNum -> 
            Array.take 1 bytes
        | 1 when Array.exists ((=) bytes.[0]) DataType.NegativeFixNum -> 
            Array.take 1 bytes
        | 1 -> [| yield DataType.Int8; yield! Array.take 1 bytes |]
        | 2 when Array.get bytes 0 < 128uy && Array.get bytes 1 = 0uy -> pack 1
        | 2 when Array.get bytes 0 > 127uy && Array.get bytes 1 = 255uy -> pack 1
        | 2 -> [| yield DataType.Int16; yield! Array.take 2 bytes |]
        | x when Array.get bytes (x - 1) = 0uy && Array.get bytes (x - 2) = 0uy -> pack (x - 2)
        | x when Array.get bytes (x - 1) = 255uy && Array.get bytes (x - 2) = 255uy -> pack (x - 2)
        | 4 -> [| yield DataType.Int32; yield! Array.take 4 bytes |]
        | 8 -> [| yield DataType.Int64; yield! Array.take 8 bytes |]
        | _ -> failwith "Cannot happen. failing packing int"
        
    Array.length bytes |> pack

let private packString (str : string) =
    let bytes = Encoding.UTF8.GetBytes str
    match Array.length bytes with
    | cnt when cnt < 32 -> 
        [| yield DataType.FixString.[cnt]
           yield! bytes |] 
    | cnt when cnt < pown 2 8 ->
        [| yield DataType.String8
           yield uint8 cnt
           yield! bytes |] 
    | cnt when cnt < pown 2 16 ->
        [| yield DataType.String16
           yield! uint16 cnt |> BitConverter.GetBytes
           yield! bytes |] 
    | cnt when int64 cnt < pown 2L 32 ->
        [| yield DataType.String32
           yield! uint32 cnt |> BitConverter.GetBytes
           yield! bytes |]
    | _ -> failwith "Value not supported"

let private packBinary bytes =
    match Array.length bytes with
    | cnt when cnt < pown 2 8 ->
        [| yield DataType.Binary8
           yield uint8 cnt
           yield! bytes |]
    | cnt when cnt < pown 2 16 ->
        [| yield DataType.Binary16
           yield! uint16 cnt |> BitConverter.GetBytes
           yield! bytes |]
    | cnt when int64 cnt < pown 2L 32 ->
        [| yield DataType.Binary32
           yield! cnt |> BitConverter.GetBytes
           yield! bytes |] 
    | _ -> failwith "Value not supported"

let private packExtension t bytes =
    if t < 0y || t > 127y then failwith "Extension type must be between 0 and 127"
    
    let typeByte = byte t
    match Array.length bytes with
    | 1 -> [| yield DataType.FixExtension1
              yield typeByte
              yield! bytes |]
    | 2 -> [| yield DataType.FixExtension2
              yield typeByte
              yield! bytes |]
    | 4 -> [| yield DataType.FixExtension4
              yield typeByte
              yield! bytes |]
    | 8 -> [| yield DataType.FixExtension8
              yield typeByte
              yield! bytes |]
    | 16 -> [| yield DataType.FixExtension16
               yield typeByte
               yield! bytes |]
    | cnt when cnt < pown 2 8 -> 
            [| yield DataType.Extension8
               yield uint8 cnt
               yield typeByte
               yield! bytes |]
    | cnt when cnt < pown 2 16 -> 
            [| yield DataType.Extension16
               yield! uint16 cnt |> BitConverter.GetBytes
               yield typeByte
               yield! bytes |]
    | cnt when int64 cnt < pown 2L 32 -> 
            [| yield DataType.Extension32
               yield! cnt |> BitConverter.GetBytes
               yield typeByte
               yield! bytes |]
    | _ -> failwith "Value not supported"

let rec pack = function
    | Value.Nil -> [| DataType.Nil |]
    | Value.Bool b when b -> [| DataType.True |]
    | Value.Bool _ -> [| DataType.False |]
    | Value.UInt8 b -> packUInt [| b |]
    | Value.UInt16 i -> i |> BitConverter.GetBytes |> packUInt
    | Value.UInt32 i -> i |> BitConverter.GetBytes |> packUInt
    | Value.UInt64 i -> i |> BitConverter.GetBytes |> packUInt
    | Value.Int8 b -> packInt [| byte b |]
    | Value.Int16 i -> i |> BitConverter.GetBytes |> packInt
    | Value.Int32 i -> i |> BitConverter.GetBytes |> packInt
    | Value.Int64 i -> i |> BitConverter.GetBytes |> packInt
    | Value.Float32 f -> [| yield DataType.Float32; yield! BitConverter.GetBytes f |]
    | Value.Float64 f -> [| yield DataType.Float64; yield! BitConverter.GetBytes f |]
    | Value.String s -> packString s
    | Value.Binary b -> packBinary b
    | Value.Array a -> packArray a
    | Value.Map m -> packMap m
    | Value.Extension(t, d) -> packExtension t d

and packArray a =
    let arrValues = Array.collect pack a

    match Array.length a with
    | cnt when cnt < 16 ->
        [| yield DataType.FixArray.[cnt]
           yield! arrValues |]
    | cnt when cnt < pown 2 16 ->
        [| yield DataType.Array16
           yield! uint16 cnt |> BitConverter.GetBytes
           yield! arrValues |]
    | cnt when int64 cnt < pown 2L 32 ->
        [| yield DataType.Array32
           yield! cnt |> BitConverter.GetBytes
           yield! arrValues |]
    | _ -> failwith "Value not supported"

and packMap (m : Map<Value, Value>) =
    let mapValues = Map.toArray m |> Array.collect (fun (k, v) -> Array.append (pack k) (pack v))

    match m.Count with
    | cnt when cnt < 16 ->
        [| yield DataType.FixMap.[cnt]
           yield! mapValues |] 
    | cnt when cnt < pown 2 16 ->
        [| yield DataType.Map16
           yield! uint16 cnt |> BitConverter.GetBytes
           yield! mapValues |]
    | cnt when int64 cnt < pown 2L 32 ->
        [| yield DataType.Map32
           yield! cnt |> BitConverter.GetBytes
           yield! mapValues |] 
    | _ -> failwith "Value not supported"
