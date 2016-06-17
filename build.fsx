// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Testing

// Directories
let buildDir  = "./build/"
let testDir  = "./test/"
let deployDir = "./deploy/"


// Filesets
let appReferences  = !! "/src/MsgPack/*.fsproj"
let testReferences  = !! "/src/MsgPack.Test/*.fsproj"

// version info
let version = "0.2"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Build" (fun _ ->
    MSBuildDebug buildDir "Build" appReferences
        |> Log "AppBuild-Output: "
)

Target "BuildTest" (fun _ ->
    MSBuildDebug testDir "Build" testReferences
        |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! (testDir + "/*.Test.dll")
    |> NUnit3 (fun p -> 
        {p with 
            ShadowCopy  = false
            ToolPath = "packages/test/NUnit.ConsoleRunner/tools/nunit3-console.exe" })
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
        -- "*.zip"
        |> Zip buildDir (deployDir + "ApplicationName." + version + ".zip")
)

// Build order
"Clean"
  ==> "Build"
  ==> "BuildTest"
  ==> "Test"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
