[<AutoOpen>]
module MsgPack.Values

type Value =
    | Nil
    | Bool of bool
    | Int8 of sbyte | Int16 of int16 | Int32 of int | Int64 of int64
    | UInt8 of byte | UInt16 of uint16 | UInt32 of uint32 | UInt64 of uint64
    | Float32 of float32 | Float64 of float
    | String of string
    | Binary of byte[]
    | Array of Value[]
    | Map of Map<Value, Value>
    | Extension of int8 * byte[]

let (|UInt8|_|) = function Value.UInt8 b -> Some b | _ -> None

let (|UInt16|_|) = function 
    | UInt8 b -> uint16 b |> Some
    | Value.UInt16 i -> Some i
    | _ -> None
let (|UInt32|_|) = function 
    | UInt16 i -> uint32 i |> Some
    | Value.UInt32 i -> Some i
    | _ -> None
let (|UInt64|_|) = function 
    | UInt32 i -> uint64 i |> Some
    | Value.UInt64 i -> Some i
    | _ -> None
    
let (|Int8|_|) = function 
    | Value.Int8 b -> Some b
    | Value.UInt8 b -> int8 b |> Some // FixNum positive
    | _ -> None

let (|Int16|_|) = function 
    | Int8 b -> int16 b |> Some
    | Value.Int16 i -> Some i
    | _ -> None
let (|Int32|_|) = function
    | Int16 i -> int32 i |> Some
    | Value.Int32 i -> Some i
    | _ -> None
let (|Int64|_|) = function 
    | Int32 i -> int64 i |> Some
    | Value.Int64 i -> Some i
    | _ -> None

let (|Float32|_|) = function Value.Float32 f -> Some f | _ -> None
let (|Float64|_|) = function 
    | Float32 f -> float f |> Some
    | Value.Float64 f -> Some f
    | _ -> None
