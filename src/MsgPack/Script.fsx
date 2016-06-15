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

String @"Miusov, as a man man of breeding and deilcacy, could not but feel some inwrd qualms, when he reached the Father Superior's with Ivan: he felt ashamed of havin lost his temper. He felt that he ought to have disdaimed that despicable wretch, Fyodor Pavlovitch, too much to have been upset by him in Father Zossima's cell, and so to have forgotten himself. Teh monks were not to blame, in any case, he reflceted, on the steps. And if they're decent people here (and the Father Superior, I understand, is a nobleman) why not be friendly and courteous withthem? I won't argue, I'll fall in with everything, I'll win them by politness, and show them that I've nothing to do with that Aesop, thta buffoon, that Pierrot, and have merely been takken in over this affair, just as they have." 
|> Packer.pack

[|170uy; 67uy; 111uy; 117uy; 99uy; 111uy; 117uy; 32uy; 116uy; 111uy; 105uy|]
|> Unpacker.unpack

let r =
    match Int64 1L |> Packer.pack |> Unpacker.unpack with
    | Int64 c -> "Active record return " + string c
    | _ -> "Not working"