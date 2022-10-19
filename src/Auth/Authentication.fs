namespace Auth 

module Authentication = 
    open Shared.Types 

    let customerRole = "Customer"
    let customerAgentRole = "CustomerAgent"

    let makePrincipal (name:string) (role:string) = 
        {
            Name = name
            Roles = [| role |]
        }


    let authenticate name = 
        match name with
        | "luis" | "maxime" -> 
            makePrincipal name customerRole  |> Ok
        | "sean" -> 
            makePrincipal name customerAgentRole |> Ok
        | _ -> 
            AuthenticationFailed name |> Error 

    let customerIdForName name = 
        match name with
        | "luis" -> CustomerId 1 |> Ok
        | "maxime" -> CustomerId 2 |> Ok
        | "sean" -> CustomerId 3 |> Ok
        | _ -> CustomerNameNotFound name |> Error

    let orElse errValue = function
        | Ok x -> x
        | Error _ -> errValue

    let customerIdOwnedByPrincipal customerId principal = 
        principal.Name
        |> customerIdForName 
        |> Result.map (fun principalId -> principalId = customerId)
        |> orElse false
    
    let userIsOwnedByPrincipal principal =
        //Compare principal.Name against databse
        true