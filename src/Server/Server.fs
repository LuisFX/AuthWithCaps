module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open BusinessLayer
open Shared.Capabilities
open Shared.Types
open Shared

// module CapabilityEndpoints =
//     let RouteBuilder =


// module Storage =
//     let todos = ResizeArray()

//     let addTodo (todo: Todo) =
//         if Todo.isValid todo.Description then
//             todos.Add todo
//             Ok()
//         else
//             Error "Invalid todo"

//     do
//         addTodo (Todo.create "Create new SAFE project")
//         |> ignore

//         addTodo (Todo.create "Write your app") |> ignore
//         addTodo (Todo.create "Ship it !!!") |> ignore

let serverApi =
    {
        getTodos = fun userId ->
            let u = {
                Name = "user1"
                Roles = [| "admin" |]
            }
            async {
                let! a = Capabilities.allCapabilities.getTodos userId u
                match a with
                | Some cap ->
                    match cap() with
                    | Ok l ->
                        return l
                    | Error err ->
                        return []
                | None -> return []
            }
    } : IApiCapabilityProvider

let webApp : Giraffe.Core.HttpFunc -> Microsoft.AspNetCore.Http.HttpContext -> Giraffe.Core.HttpFuncResult =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler

let app =
    application {
        use_router webApp
        // use_router webAppCapabilities
        memory_cache
        use_static "public"
        use_gzip
    }

[<EntryPoint>]
let main _ =
    run app
    0