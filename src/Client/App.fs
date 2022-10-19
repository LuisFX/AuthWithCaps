module App

open Feliz
open Fable.Remoting.Client
open Shared

[<ReactComponent>]
let App () =
    Html.div "Hello World"




// module App

// open Elmish
// open Elmish.React

// #if DEBUG
// open Elmish.Debug
// open Elmish.HMR
// #endif

// Program.mkProgram Index.init Index.update Index.view
// #if DEBUG
// |> Program.withConsoleTrace
// #endif
// |> Program.withReactSynchronous "elmish-app"
// #if DEBUG
// |> Program.withDebugger
// #endif
// |> Program.run