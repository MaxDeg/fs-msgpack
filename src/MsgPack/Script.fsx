#load "Value.fs"
#load "DataType.fs"
#load "Packer.fs"
#load "Unpacker.fs"

open MsgPack

let values = Array [| Nil; Int32 -30; Bool true; Int32 120; UInt32 254u; Binary [| 0uy; 1uy; 2uy |] |] |> Packer.pack
let encode = System.Convert.ToBase64String

Unpacker.unpack values

encode values


Extension(1y, [| 0uy; 1uy; 2uy |])
|> Packer.pack
|> Unpacker.unpack