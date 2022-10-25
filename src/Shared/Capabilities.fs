namespace Shared
open Elmish
module Capabilities =
    open Types
    open Shared.Types
    open System.Collections.Generic

    // each access token gets its own type
    type AccessCustomer = AccessCustomer of CustomerId
    type UpdatePassword = UpdatePassword of CustomerId
    type AccssTodos = AccssTodos of User

    // capabilities
    type GetCustomerCap = unit -> Result<CustomerData,FailureCase>
    type UpdateCustomerCap = CustomerData -> Result<unit,FailureCase>
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
        getCustomer : CustomerId -> User -> Async<GetCustomerCap option>
        /// given a customerId and User, attempt to get the UpdateCustomer capability
        updateCustomer : CustomerId -> User -> Async<UpdateCustomerCap option>
        /// given a customerId and User, attempt to get the UpdatePassword capability
        updatePassword : CustomerId -> User -> Async<UpdatePasswordCap option>
        /// get all todos for the given user
        getTodos : User -> Async<GetTodosCap option>
    }

    type IUICapabilityProvider = {
        getTodos: User -> (string list -> Msg) -> (exn -> Msg) -> FetchTodoCap option
        getTodos2: User -> FetchTodoCap option
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