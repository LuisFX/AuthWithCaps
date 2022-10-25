namespace Auth
open System

module Authorization =
    open Shared.Types
    open Shared.Capabilities

    // the constructor is protected
    type AccessToken<'data> = private {data:'data} with
        // but do allow read access to the data
        member this.Data = this.data

    let onlyForSameId (id:CustomerId) principal =
        if Authentication.customerIdOwnedByPrincipal id principal then
            Some {data=AccessCustomer id}
        else
            None

    let onlyForAgents (id:CustomerId) principal  =
        if principal.Roles |> Array.contains Authentication.customerAgentRole then
            Some {data=AccessCustomer id}
        else
            None

    let onlyIfDuringBusinessHours (time:DateTime) f =
        if time.Hour >= 8 && time.Hour <= 17 then
            Some f
        else
            None

    // constrain who can call a password update function
    let passwordUpdate (id:CustomerId) principal =
        if Authentication.customerIdOwnedByPrincipal id principal then
            Some {data=UpdatePassword id}
        else
            None

    let todosAccssForUser principal =
        if Authentication.userIsOwnedByPrincipal principal then
            Some {data=AccssTodos principal}
        else
            None

    // return the first good capability, if any
    let first capabilityList =
        capabilityList |> List.tryPick id

    // given a capability option, restrict it
    let restrict filter originalCap =
        originalCap
        |> Option.bind filter

    /// Uses of the capability will be audited
    let auditable capabilityName principalName f =
        fun x ->
            // simple audit log!
            let timestamp = DateTime.UtcNow.ToString("u")
            printfn "AUDIT: User %s used capability %s at %s" principalName capabilityName timestamp
            // use the capability
            f x

    /// Return a pair of functions: the revokable capability,
    /// and the revoker function
    let revokable f =
        let allow = ref true
        let capability = fun x ->
            if !allow then  //! is dereferencing not negation!
                f x
            else
                Error CapabilityRevoked
        let revoker() =
            allow := false
        capability, revoker