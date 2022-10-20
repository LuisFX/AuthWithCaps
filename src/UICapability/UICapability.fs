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

    let allCapabilities = 
        let getTodosOnlyForUser (principal:User) =
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
            getTodos = getTodosOnlyForUser //User -> option<GetTodosCap>
        } : IUICapabilityProvider

