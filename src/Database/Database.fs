namespace Database
open System.Collections.Generic
open Shared.Types 
open Shared.Capabilities
open Auth.Authorization

module CustomerDatastore = 

    let private db = 
        let db = Dictionary<CustomerId,CustomerData>()
        db.Add(CustomerId 1, CustomerData "Data for customer 1")
        db.Add(CustomerId 2, CustomerData "Data for customer 2")
        db

    let getCustomer (accessToken:AccessToken<AccessCustomer>) = 
        // get customer id
        let (AccessCustomer id) = accessToken.Data

        // now get customer data using the id
        match db.TryGetValue id with
        | true, value -> Ok value 
        | false, _ -> Error (CustomerIdNotFound id)

    let updateCustomer (accessToken:AccessToken<AccessCustomer>) (data:CustomerData) = 
        // get customer id
        let (AccessCustomer id) = accessToken.Data

        // update database
        db.[id] <- data
        Ok()

    let updatePassword (accessToken:AccessToken<UpdatePassword>) (password:Password) = 
        Ok()   // dummy implementation

module TodoDataStore =
    let getTodos (accessToken:AccessToken<AccssTodos>) principal =
        let (AccssTodos user) = accessToken.Data
        if user = user then
            Ok ["one"; "two"; "three"]
        else
            Error NotAllowedToGetTodos
