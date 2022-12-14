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

    let clientApi : IApiCapabilityProvider =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<Capabilities.IApiCapabilityProvider>

    let allCapabilities  =
        let getTodosOnlyForUser (id:UserId) (principal:UserPrincipal) =
            let accessToken : AccessToken<AccssTodos> option = Authorization.todosAccssForUser id principal
            if Authentication.userIdOwnedByPrincipal id principal then
                accessToken
                |> tokenToCap2 (fun accessToken ->
                    let (AccssTodos userId) = accessToken.Data
                    // if userId = principal then
                    clientApi.getTodos
                    // else
                        // Cmd.none
                )
            else
                None
        // let getTodosOnlyForUser2 (id:UserId) principal =
        //     let accessToken : AccessToken<AccssTodos> option = Authorization.todosAccssForUser id principal
        //     accessToken
        //     |> tokenToCap2 (fun accessToken _ ->
        //         let (AccssTodos user) = accessToken.Data
        //         // if user = principal then
        //         Cmd.OfAsyncWith.either Async.StartImmediate api.getTodos userId GotTodos GotTodosError
        //         // else
        //             // Cmd.none
        //     )

        // create the record that contains the capabilities
        {
            getTodos = getTodosOnlyForUser
            getTodos2 = getTodosOnlyForUser
            // getTodos2 = getTodosOnlyForUser2
        } : IUICapabilityProvider

    let mainPageCaps userId principal =
        {|
            getTodos1 = (allCapabilities).getTodos userId principal
            getTodos2 = (allCapabilities).getTodos2 userId principal
        |}

// let inline square (x: ^a when ^a: (static member (*): ^a -> ^a -> ^a)) = x * x
// let inline dupe (x: ^a when ^a: (static member (+): ^a -> ^a -> ^a)) = x + x

// let aa = square 3
// let ab = dupe "s"
