namespace Shared

open System

type Todo = {
        Id: Guid; Description: string
    }

module Todo =
    let isValid (description: string) =
        String.IsNullOrWhiteSpace description |> not

    let create (description: string) = {
        Id = Guid.NewGuid()
        Description = description
    }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

    let capabilityRouteBuilder _ methodName =
        printf "Route is: %s" methodName
        sprintf "/api/capability/%s" methodName

type ITodosApi = {
        getTodos: unit -> Async<string list>
        // addTodo: Todo -> Async<Todo>
    }