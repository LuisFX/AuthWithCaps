module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open BusinessLayer
open Shared
open Shared.Types

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

let todosApi =
    { 
        getTodos = fun () -> 
            async {
                let user = {
                    Name = "luis"
                    Roles = [| "Customer" |]
                }
                let! a = Capabilities.allCapabilities.getTodos user
                match a with
                | Some cap -> 
                    match BusinessLayer.Logic.getTodos cap with
                    | Ok l -> 
                        return l
                    | Error err ->
                        return []
                | None -> return []
            }
    }

let webApp : Giraffe.Core.HttpFunc -> Microsoft.AspNetCore.Http.HttpContext -> Giraffe.Core.HttpFuncResult =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler

// let webAppCapabilities: Giraffe.Core.HttpFunc -> Microsoft.AspNetCore.Http.HttpContext -> Giraffe.Core.HttpFuncResult =
//     Remoting.createApi ()
//     |> Remoting.withRouteBuilder Route.capabilityRouteBuilder
//     |> Remoting.fromValue Capabilities.allCapabilities
//     |> Remoting.buildHttpHandler

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