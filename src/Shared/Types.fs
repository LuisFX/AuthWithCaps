namespace Shared

module Types =

    type User = { Name: string; Roles: string[] }
    type CustomerId = CustomerId of int
    type CustomerData = CustomerData of string
    type Password = Password of string

    type FailureCase = 
        | AuthenticationFailed of string
        | AuthorizationFailed
        | CustomerNameNotFound of string
        | CustomerIdNotFound of CustomerId
        | OnlyAllowedOnce
        | CapabilityRevoked
        | NotAllowedToGetTodos