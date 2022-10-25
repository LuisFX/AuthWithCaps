namespace Shared

module Types =

    type User = { Name: string; Roles: string[] }
    type CustomerId = CustomerId of int
    type CustomerData = CustomerData of string
    type Password = Password of string

    // type TryValue<'a,'b> =
    //     | Value of 'a
    //     | Error of 'b
    type Msg =
        | Login of string * string
        | GetTodos
        | GotTodos of string list // this would actually be when server
        | GotTodosError of exn
        | SelectCustomer of User * string
        | Logout

    type FailureCase =
        | AuthenticationFailed of string
        | AuthorizationFailed
        | CustomerNameNotFound of string
        | CustomerIdNotFound of CustomerId
        | OnlyAllowedOnce
        | CapabilityRevoked
        | NotAllowedToGetTodos