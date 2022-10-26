namespace Database
open System.Collections.Generic
open Shared.Types
open Shared.Capabilities
open Auth.Authorization
open System

module Store =
    let db =
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
module CustomerDatastore =



    let getCustomer (accessToken:AccessToken<AccessCustomer>) =
        // get customer id
        let (AccessCustomer id) = accessToken.Data

        // now get customer data using the id
        match Store.db.TryGetValue id with
        | true, value -> Ok value
        | false, _ -> Error (UserIdNotFound id)

    let updateCustomer (accessToken:AccessToken<AccessCustomer>) (data:UserData) =
        // get customer id
        let (AccessCustomer id) = accessToken.Data

        // update database
        Store.db.[id] <- data
        Ok()

    let updatePassword (accessToken:AccessToken<UpdatePassword>) (password:Password) =
        Ok()   // dummy implementation

module TodoDataStore =
    let getTodos (accessToken:AccessToken<AccssTodos>) =
        let (AccssTodos user) = accessToken.Data
        match Store.db.TryGetValue user with
        | true, value -> Ok value.Todos
        | false, _ -> Error ( NotAllowedToGetTodos)
        // else
        //     Error NotAllowedToGetTodos
