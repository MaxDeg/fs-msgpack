module MsgPack.Test.Packer

open MsgPack
open MsgPack.Packer
open NUnit.Framework
open FsUnit

[<Test>]
let ``Pack Nil`` () =
    pack Nil |> should equal [| 0xc0uy |]

[<Test>]
let ``Pack True`` () =
    Bool true |> pack |> should equal [| 0xc3uy |]
    
[<Test>]
let ``Pack False`` () =
    Bool false |> pack |> should equal [| 0xc2uy |]
    
module Integer =
    
    [<Test>]
    let ``Pack 2 as fixnum`` () = 
        Int8 2y |> pack |> should equal [| 0x02uy |]
    
    [<Test>]
    let ``Pack 127 as fixnum`` () = 
        Int8 127y |> pack |> should equal [| 0x7fuy |]
        
    [<Test>]
    let ``Pack 128 as Int8`` () = 
        Int8 (int8 128) |> pack |> should equal [| 0xd0uy; 128uy |]
        
    [<Test>]
    let ``Pack -127 as Int8`` () = 
        Int8 (int8 -127) |> pack |> should equal [| 0xd0uy; 129uy |]
        
    [<Test>]
    let ``Pack Int16 128 as Int8`` () = 
        Int16 (int16 128) |> pack |> should equal [| 0xd0uy; 128uy |]
        
    [<Test>]
    let ``Pack 256 as Int16`` () = 
        Int16 (int16 256) |> pack |> should equal [| 0xd1uy; 0x00uy; 0x01uy |]
