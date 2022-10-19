module ClientCapability

open System
open Shared.Types
open Auth
open Shared.Capabilities

module Capabilities =
    open Auth.Authorization

    let allCapabilities = 
        let getTodosOnlyForUser (principal:User) =
            let accessToken : AccessToken<AccssTodos> option = Authorization.todosAccssForUser principal
            accessToken
            |> tokenToCap2 (fun accessToken _ ->
                let (AccssTodos user) = accessToken.Data
                if user.Name = principal.Name then
                    Ok []
                else
                    Error NotAllowedToGetTodos
            )

        // create the record that contains the capabilities
        {
            getCustomer = failwith "not implemented"
            updateCustomer = failwith "not implemented"
            updatePassword = failwith "not implemented"
            getTodos = getTodosOnlyForUser //User -> option<GetTodosCap>
        } : ICapabilityProvider