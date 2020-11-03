#r "paket:
nuget FSharp.Core
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fake.DotNet.Cli //"
#load "./.fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.Globbing.Operators

let identityServerPath = Path.getFullName "./src/IdentityServer"
let apiServerPath = Path.getFullName "./src/Api"

let npm args workingDir =
    let npmPath =
        match ProcessUtils.tryFindFileOnPath "npm" with
        | Some path -> path
        | None ->
            "npm was not found in path. Please install it and make sure it's available from your path. " +
            "See https://safe-stack.github.io/docs/quickstart/#install-pre-requisites for more info"
            |> failwith

    let arguments = args |> String.split ' ' |> Arguments.OfArgs

    Command.RawCommand (npmPath, arguments)
    |> CreateProcess.fromCommand
    |> CreateProcess.withWorkingDirectory workingDir
    |> CreateProcess.ensureExitCode
    |> Proc.run
    |> ignore

let dotnet cmd workingDir =
    let result = Fake.DotNet.DotNet.exec (Fake.DotNet.DotNet.Options.withWorkingDirectory workingDir) cmd ""
    if result.ExitCode <> 0 then failwithf "'dotnet %s' failed in %s" cmd workingDir

// *** Define Targets ***
Target.create "Clean" (fun _ ->
  Trace.log " --- Cleaning stuff --- "

  Shell.rm_rf "node_modules"

  GlobbingPattern.createFrom "public" ++ "**/*.js" 
  |> Seq.iter Shell.rm_rf

  dotnet "clean" "."
)

Target.create "Build" (fun _ ->
  Trace.log " --- Building the app --- "
  dotnet "build" "."
  npm "install" "."
)

Target.create "Run" (fun _ ->
  Trace.log " --- Running the app --- "
  [ async { dotnet "watch run" identityServerPath }
    async { dotnet "watch run" apiServerPath }
    async { npm "start" "." } ]
    |> Async.Parallel
    |> Async.RunSynchronously
    |> ignore
)

open Fake.Core.TargetOperators

// *** Define Dependencies ***
"Clean"
  ==> "Build"
  ==> "Run"

// *** Start Build ***
Target.runOrDefault "Run"

