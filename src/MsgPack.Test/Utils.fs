namespace MsgPack.Test
[<AutoOpen>]
module Utils =
    open FsUnit

    type InitMsgUtils() =
        inherit FSharpCustomMessageFormatter()