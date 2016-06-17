module MsgPack.Unpacker

open System
open System.Text

let private unpackUInt8 tail =
    let value = Array.head tail
    Value.UInt8 value, Array.tail tail
    
let private unpackUInt16 tail =
    let values = Array.take 2 tail
    BitConverter.ToUInt16(values, 0) |> Value.UInt16,
    Array.skip 2 tail
    
let private unpackUInt32 tail =
    let values = Array.take 4 tail
    BitConverter.ToUInt32(values, 0) |> Value.UInt32,
    Array.skip 4 tail
    
let private unpackUInt64 tail =
    let values = Array.take 8 tail
    BitConverter.ToUInt64(values, 0) |> Value.UInt64,
    Array.skip 8 tail
        
let private unpackInt8 tail =
    let value = Array.head tail
    Value.Int8 (sbyte value),
    Array.tail tail
    
let private unpackInt16 tail =
    let values = Array.take 2 tail
    BitConverter.ToInt16(values, 0) |> Value.Int16,
    Array.skip 2 tail
    
let private unpackInt32 tail =
    let values = Array.take 4 tail
    BitConverter.ToInt32(values, 0) |> Value.Int32,
    Array.skip 4 tail
    
let private unpackInt64 tail =
    let values = Array.take 8 tail
    BitConverter.ToInt64(values, 0) |> Value.Int64,
    Array.skip 8 tail

let private unpackFloat32 tail =
    BitConverter.ToSingle(tail, 0) |> Value.Float32,
    Array.skip 4 tail
        
let private unpackFloat64 tail =
    BitConverter.ToDouble(tail, 0) |> Value.Float64,
    Array.skip 8 tail

let private unpackString tail size =
    tail |> Array.take size |> Encoding.UTF8.GetString |> Value.String

let private unpackFixString head tail =
    let cnt = int(head &&& byte((1 <<< 5) - 1)) // 5-bit int
    let tail' = Array.take cnt tail
    Encoding.UTF8.GetString tail' |> Value.String,
    Array.skip cnt tail

let private unpackString8 tail =
    printfn "unpackString8"
    let cnt = Array.head tail |> int
    let tail' = Array.tail tail
    unpackString tail' cnt,
    Array.skip cnt tail'

let private unpackString16 tail =
    printfn "unpackString16"
    let cnt = BitConverter.ToUInt16(Array.take 2 tail, 0) |> int
    let tail' = Array.skip 2 tail
    unpackString tail' cnt,
    Array.skip cnt tail'

let private unpackString32 tail =
    printfn "unpackString32"
    let cnt = BitConverter.ToUInt32(Array.take 4 tail, 0) |> int
    let tail' = Array.skip 4 tail
    unpackString tail' cnt,
    Array.skip cnt tail'
        
let private unpackBinary8 tail =
    let cnt = Array.head tail |> int
    let tail' = Array.tail tail
    Array.take cnt tail' |> Value.Binary,
    Array.skip cnt tail'

let private unpackBinary16 tail =
    let cnt = BitConverter.ToUInt16(Array.take 2 tail, 0) |> int
    let tail' = Array.skip 2 tail
    Array.take cnt tail' |> Value.Binary,
    Array.skip cnt tail'

let private unpackBinary32 tail =
    let cnt = BitConverter.ToUInt32(Array.take 4 tail, 0) |> int
    let tail' = Array.skip 4 tail
    Array.take cnt tail' |> Value.Binary,
    Array.skip cnt tail'

let private unpackExtension x tail =
    let typeByte = Array.head tail |> int8
    let tail' = Array.tail tail
    Extension(typeByte, Array.take x tail'),
    Array.skip x tail'

let private unpackFixExtension1 = unpackExtension 1
let private unpackFixExtension2 = unpackExtension 2        
let private unpackFixExtension4 = unpackExtension 4        
let private unpackFixExtension8 = unpackExtension 8        
let private unpackFixExtension16 = unpackExtension 16

let private unpackExtension8 tail =
    let cnt = Array.head tail |> int
    Array.skip 1 tail |> unpackExtension cnt
    
let private unpackExtension16 tail =
    let cnt = BitConverter.ToUInt16(Array.take 2 tail, 0) |> int
    Array.skip 2 tail |> unpackExtension cnt
    
let private unpackExtension32 tail =
    let cnt = BitConverter.ToUInt32(Array.take 4 tail, 0) |> int
    Array.skip 4 tail |> unpackExtension cnt
    
let unpack bytes =
    printfn "unpack"
    let rec unpack' bytes' =
        let tail = Array.tail bytes'
        match Array.head bytes' with
        | DataType.Nil -> Value.Nil, tail
        | DataType.True -> Value.Bool true, tail
        | DataType.False -> Value.Bool false, tail
        | b when Array.exists ((=) b) DataType.PositiveFixNum -> Value.UInt8 b, tail
        | b when Array.exists ((=) b) DataType.NegativeFixNum -> Value.Int8 (sbyte b), tail
        | DataType.UInt8 -> unpackUInt8 tail
        | DataType.UInt16 -> unpackUInt16 tail
        | DataType.UInt32 -> unpackUInt32 tail
        | DataType.UInt64 -> unpackUInt64 tail
        | DataType.Int8 -> unpackInt8 tail
        | DataType.Int16 -> unpackInt16 tail
        | DataType.Int32 -> unpackInt32 tail
        | DataType.Int64 -> unpackInt64 tail
        | DataType.Float32 -> unpackFloat32 tail
        | DataType.Float64 -> unpackFloat64 tail
        | h when Array.exists ((=) h) DataType.FixString -> unpackFixString h tail
        | DataType.String8 -> unpackString8 tail
        | DataType.String16 -> unpackString16 tail
        | DataType.String32 -> unpackString32 tail
        | DataType.Binary8 -> unpackBinary8 tail
        | DataType.Binary16 -> unpackBinary16 tail
        | DataType.Binary32 -> unpackBinary32 tail
        | h when Array.exists ((=) h) DataType.FixArray -> unpackFixArray h tail
        | DataType.Array16 -> unpackArray16 tail
        | DataType.Array32 -> unpackArray32 tail
        | h when Array.exists ((=) h) DataType.FixMap -> unpackFixMap h tail
        | DataType.Map16 -> unpackMap16 tail
        | DataType.Map32 ->unpackMap32 tail
        | DataType.FixExtension1 -> unpackFixExtension1 tail
        | DataType.FixExtension2 -> unpackFixExtension2 tail
        | DataType.FixExtension4 -> unpackFixExtension4 tail
        | DataType.FixExtension8 -> unpackFixExtension8 tail
        | DataType.FixExtension16 -> unpackFixExtension16 tail
        | DataType.Extension8 -> unpackExtension8 tail
        | DataType.Extension16 -> unpackExtension16 tail
        | DataType.Extension32 -> unpackExtension32 tail
        | _ -> failwith "Unknow encoding"
    
    and unpackArray =
        let rec unpackArray' acc cnt tail =
            if cnt = 0 then
                acc |> List.rev |> List.toArray |> Value.Array, tail
            else
                let value, tail' = unpack' tail
                unpackArray' (value :: acc) (cnt - 1) tail'
        
        unpackArray' []

    and unpackFixArray head tail =
        let cnt = int(head &&& byte((1 <<< 4) - 1)) // 4-bit int
        unpackArray cnt tail

    and unpackArray16 tail =
        let cnt = BitConverter.ToUInt16(Array.take 2 tail, 0) |> int
        Array.skip 2 tail |> unpackArray cnt

    and unpackArray32 tail =
        let cnt = BitConverter.ToUInt32(Array.take 4 tail, 0) |> int
        Array.skip 4 tail |> unpackArray cnt

    and unpackMap =
        let rec unpackMap' acc cnt tail =
            if cnt = 0 then
                acc |> List.rev |> Map.ofList |> Value.Map, tail
            else
                let key, (value, tail') = unpack' tail |> fun (k, t) -> k, unpack' t
                unpackMap' ((key, value) :: acc) (cnt - 1) tail'
        
        unpackMap' []

    and unpackFixMap head tail =
        let cnt = int(head &&& byte((1 <<< 4) - 1)) // 4-bit int
        unpackMap cnt tail

    and unpackMap16 tail =
        let cnt = BitConverter.ToUInt16(Array.take 2 tail, 0) |> int
        Array.skip 2 tail |> unpackMap cnt

    and unpackMap32 tail =
        let cnt = BitConverter.ToUInt32(Array.take 4 tail, 0) |> int
        Array.skip 4 tail |> unpackMap cnt

    unpack' bytes |> fst
