module Main

open Feliz
open Feliz.UseElmish
open Browser.Dom
open Elmish

open Auth
open Shared.Types
open Shared.Capabilities
open Shared
open Fable
open Fable.Remoting.Client

let capabilityApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.capabilityRouteBuilder
    |> Remoting.buildProxy<Capabilities.ICapabilityProvider>

type CurrentState = 
    | LoggedOut
    | LoggedIn of User
    | CustomerSelected of User * CustomerId
    | GotCustomerDetails of CustomerData
    | Exit
        
let init() = LoggedOut, Cmd.none

let update msg state =
    match msg with
    | Logout ->
        LoggedOut, Cmd.none
    | GetTodos u ->
        // let cmd =
        state,
        match UICapability.Capabilities.allCapabilities.getTodos u with
        | Some cap ->
            cap()
        | None -> Cmd.none 

    | GotTodos l ->
        console.log ("Got todos:", l)
        state, Cmd.none


    | Login (n,p) ->
        match Authentication.authenticate n with
        | Ok principal -> 
            LoggedIn principal, Cmd.none
        | Error err -> 
            printfn ".. %A" err
            state, Cmd.none

    | SelectCustomer (principal, customerName) ->
        match Authentication.customerIdForName customerName with
            | Ok customerId -> 
                // found -- change state
                CustomerSelected (principal,customerId), Cmd.none
            | Error err -> 
                // not found -- stay in originalState 
                printfn ".. %A" err
                state, Cmd.none
            

[<ReactComponent>]
let App() =
    let state, dispatch = React.useElmish(init, update, [| |])
    match state with
    | LoggedOut ->
            Html.div [
            Html.h1 "Login"
            Html.input [
                prop.type' "text"
                prop.placeholder "Username"
                prop.value "luisfx"
            ]
            Html.input [
                prop.type' "password"
                prop.placeholder "Password"
                prop.value "123"
            ]
            Html.button [
                prop.text "Login"
                prop.onClick (fun _ -> Login ("luis", "123") |> dispatch)
            ]
        ]
    | LoggedIn principal ->
        Html.div [
            Html.h1 (sprintf "Logged in: %A" principal.Name )

            Html.span "Pick a customer"
            Html.br []
            Html.button [
                prop.text "Luis"
                prop.onClick (fun _ -> SelectCustomer (principal, "luis") |> dispatch)
            ]
            Html.br []
            Html.button [
                prop.text "maxime"
                prop.onClick (fun _ ->  SelectCustomer (principal, "maxime") |> dispatch)
            ]
            Html.br []
            Html.br []
            Html.button [
                prop.text "Logout"
                prop.onClick (fun _ -> Logout |> dispatch)
            ]
        ]
    | CustomerSelected (principal, customerId) ->
        Html.div [
            Html.h1 (sprintf "Logged in: %A" principal.Name )
            Html.h2 (sprintf "Customer: %A" customerId )
            Html.br []
            Html.button [ prop.text "Get Todos"; prop.onClick (fun _ -> GetTodos principal |> dispatch) ]
            Html.br []
            Html.button [
                prop.text "Deselect Customer"
                prop.onClick (fun _ -> Login (principal.Name, "") |> dispatch)
            ]
            Html.button [
                prop.text "Logout"
                prop.onClick (fun _ -> Logout |> dispatch)
            ]
        ]
    | GotCustomerDetails d ->
        Html.div (sprintf "Got customer details: %A" d)

[<EntryPoint>]
ReactDOM.render(
    App(),
    document.getElementById "root"
)