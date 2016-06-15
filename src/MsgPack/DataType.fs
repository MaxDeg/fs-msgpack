module internal MsgPack.DataType
    
let PositiveFixNum = [| 0x00uy..0x7fuy |]
let FixMap = [| 0x80uy..0x8fuy |]
let FixArray = [| 0x90uy..0x9fuy |]
let FixString = [| 0xa0uy..0xbfuy |]

[<Literal>]
let Nil = 0xc0uy
[<Literal>]
let False = 0xc2uy
[<Literal>]
let True = 0xc3uy
[<Literal>]
let Binary8 = 0xc4uy
[<Literal>] 
let Binary16 = 0xc5uy
[<Literal>] 
let Binary32 = 0xc6uy
[<Literal>]
let Extension8 = 0xc7uy
[<Literal>]
let Extension16 = 0xc8uy
[<Literal>]
let Extension32 = 0xc9uy
[<Literal>]
let Float32 = 0xcauy
[<Literal>]
let Float64 = 0xcbuy
[<Literal>]
let UInt8 = 0xccuy
[<Literal>]
let UInt16 = 0xcduy
[<Literal>]
let UInt32 = 0xceuy
[<Literal>]
let UInt64 = 0xcfuy
[<Literal>]
let Int8 = 0xd0uy
[<Literal>]
let Int16 = 0xd1uy
[<Literal>]
let Int32 = 0xd2uy
[<Literal>]
let Int64 = 0xd3uy
[<Literal>]
let FixExtension1 = 0xd4uy
[<Literal>]
let FixExtension2 = 0xd5uy
[<Literal>]
let FixExtension4 = 0xd6uy
[<Literal>]
let FixExtension8 = 0xd7uy
[<Literal>]
let FixExtension16 = 0xd8uy
[<Literal>]
let String8 = 0xd9uy
[<Literal>]
let String16 = 0xdauy
[<Literal>]
let String32 = 0xdbuy    
[<Literal>]
let Array16 = 0xdcuy
[<Literal>]
let Array32 = 0xdduy
[<Literal>]
let Map16 = 0xdeuy
[<Literal>]
let Map32 = 0xdfuy
let NegativeFixNum = [| 0xe0uy..0xffuy |]
