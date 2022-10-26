namespace Auth

module Authentication =
    open Shared.Types

    let userRole = "User"
    let adminRole = "Admin"

    let makePrincipal (name:string) (role:string) =
        {
            Name = name
            Roles = [| role |]
        }


    let authenticate name =
        match name with
        | "luis" | "maxime" ->
            makePrincipal name userRole  |> Ok
        | "sean" ->
            makePrincipal name adminRole |> Ok
        | _ ->
            AuthenticationFailed name |> Error

    let userIdForName name =
        match name with
        | "luis" -> UserId 1 |> Ok
        | "maxime" -> UserId 2 |> Ok
        | "sean" -> UserId 3 |> Ok
        | _ -> UserNameNotFound name |> Error

    let orElse errValue = function
        | Ok x -> x
        | Error _ -> errValue

    let userIdOwnedByPrincipal userId principal =
        principal.Name
        |> userIdForName
        |> Result.map (fun principalId -> principalId = userId)
        |> orElse false

    let userIsOwnedByPrincipal principal =
        //Compare principal.Name against databse
        true