namespace Shared
open System

module Types =

    type UserPrincipal = { Name: string; Roles: string[] }
    type UserId = UserId of int

    type Appointment = { Date: DateTime; Description: string }
    type Todo = { Description: string; Completed: bool }

    type UserData = {
        Todos: Todo list
        Appointments:  Appointment list
    }
    type Password = Password of string

    type FailureCase =
        | AuthenticationFailed of string
        | AuthorizationFailed
        | UserNameNotFound of string
        | UserIdNotFound of UserId
        | OnlyAllowedOnce
        | CapabilityRevoked
        | NotAllowedToGetTodos