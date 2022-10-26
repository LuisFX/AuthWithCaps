namespace Database
open System.Collections.Generic
open Shared.Types
open Shared.Capabilities
open Auth.Authorization
open System

module CustomerDatastore =

    let private db =
        let db = Dictionary<UserId, UserData>()
        db.Add( UserId 1,
            {
                Todos =
                    [
                        {
                            Description = "Todo 1"
                            Completed = false
                        }
                    ]
                Appointments =
                    [
                        {
                            Date = DateTime.Now
                            Description = "Appointment 1"
                        }
                    ]
            }
        )
        db.Add( UserId 2,
            {
                Todos =
                    [
                        {
                            Description = "Todo A"
                            Completed = false
                        }
                    ]
                Appointments =
                    [
                        {
                            Date = DateTime.Now
                            Description = "Appointment A"
                        }
                    ]
            }
        )
        db

    let getCustomer (accessToken:AccessToken<AccessCustomer>) =
        // get customer id
        let (AccessCustomer id) = accessToken.Data

        // now get customer data using the id
        match db.TryGetValue id with
        | true, value -> Ok value
        | false, _ -> Error (UserIdNotFound id)

    let updateCustomer (accessToken:AccessToken<AccessCustomer>) (data:UserData) =
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
