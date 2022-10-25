module UICapability

open System
open Shared.Types
open Auth
open Shared.Capabilities
open Fable.Remoting.Client
open Shared
open Elmish
module Capabilities =
    open Auth.Authorization

    let api : IApiCapabilityProvider =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<Capabilities.IApiCapabilityProvider>

    let allCapabilities  =
        let getTodosOnlyForUser principal (success:string list -> Msg) (failure:exn -> Msg) =
            let accessToken : AccessToken<AccssTodos> option = Authorization.todosAccssForUser principal
            accessToken
            |> tokenToCap2 (fun accessToken _ ->
                let (AccssTodos user) = accessToken.Data
                if user.Name = principal.Name then
                    Cmd.OfAsyncWith.either Async.StartImmediate api.getTodos () success failure
                else
                    Cmd.none
            )
        let getTodosOnlyForUser2 principal =
            let accessToken : AccessToken<AccssTodos> option = Authorization.todosAccssForUser principal
            accessToken
            |> tokenToCap2 (fun accessToken _ ->
                let (AccssTodos user) = accessToken.Data
                if user.Name = principal.Name then
                    Cmd.OfAsyncWith.either Async.StartImmediate api.getTodos () GotTodos GotTodosError
                else
                    Cmd.none
            )

        // create the record that contains the capabilities
        {
            getTodos = getTodosOnlyForUser
            getTodos2 = getTodosOnlyForUser2
        } : IUICapabilityProvider

    let mainPageCaps principal =
        {|
            getTodos1 = (allCapabilities).getTodos principal
            getTodos2 = (allCapabilities).getTodos2 principal
        |}

// let inline square (x: ^a when ^a: (static member (*): ^a -> ^a -> ^a)) = x * x
// let inline dupe (x: ^a when ^a: (static member (+): ^a -> ^a -> ^a)) = x + x

// let aa = square 3
// let ab = dupe "s"
