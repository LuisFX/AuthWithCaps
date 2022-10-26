namespace Shared
open Elmish
module Capabilities =
    open Types
    open Shared.Types
    open System.Collections.Generic

    // each access token gets its own type
    type AccessCustomer = AccessCustomer of UserId
    type UpdatePassword = UpdatePassword of UserId
    type AccssTodos = AccssTodos of UserId

    // capabilities
    type GetCustomerCap = unit -> Result<UserData,FailureCase>
    type UpdateCustomerCap = UserData -> Result<unit,FailureCase>
    type UpdatePasswordCap = Password -> Result<unit,FailureCase>
    type GetTodosCap = unit -> Result<string list,FailureCase>
    type FetchTodoCap = unit -> Cmd<Msg>

    // type GetClientTodosCap = unit -> Result<CustomerData,FailureCase>

    // type IClientCapabilityProvider = {
    //     // getCustomer: AccessCustomer -> GetCustomerCap
    //     // updateCustomer: AccessCustomer -> UpdateCustomerCap
    //     // updatePassword: UpdatePassword -> UpdatePasswordCap
    //     getTodos: unit -> Async<GetClientTodosCap>
    //     // unit -> Async<Todo list>
    // }

    type ICapabilityProvider = {
        /// given a customerId and User, attempt to get the GetCustomer capability
        getCustomer : UserId -> UserPrincipal -> Async<GetCustomerCap option>
        /// given a customerId and User, attempt to get the UpdateCustomer capability
        updateCustomer : UserId -> UserPrincipal -> Async<UpdateCustomerCap option>
        /// given a customerId and User, attempt to get the UpdatePassword capability
        updatePassword : UserId -> UserPrincipal -> Async<UpdatePasswordCap option>
        /// get all todos for the given user
        getTodos : UserId -> UserPrincipal -> Async<GetTodosCap option>
    }

    type IUICapabilityProvider = {
        getTodos: UserId -> UserPrincipal -> (string list -> Msg) -> (exn -> Msg) -> FetchTodoCap option
        getTodos2: UserId -> UserPrincipal -> FetchTodoCap option
    }

    type IApiCapabilityProvider = {
        getTodos: unit -> Async<string list>
    }

    // apply the token, if present,
    // to a function which has only the token as a parameter
    let tokenToCap f token =
        token
        |> Option.map (fun token ->
            fun () -> f token)

    // apply the token, if present,
    // to a function which has the token and other parameters
    let tokenToCap2 f token =
        token
        |> Option.map (fun token ->
            fun x -> f token x)