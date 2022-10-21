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


type Authenticated =
    | LoggedIn of User
    | CustomerSelected of User * CustomerId

type State = 
    | LoggedOut
    | Authenticated of Authenticated * option<FetchTodoCap>
        
let init() = LoggedOut, Cmd.none

let update msg state =
    match state, msg with
    | _, Logout ->
        LoggedOut, Cmd.none
        
    | Authenticated (a,c), GetTodos ->
        state,
        match c with
        | Some cap -> cap()
        | None -> Cmd.none

    | _, GotTodos l ->
        console.log ("Got todos:", l)
        state, Cmd.none

    | _, Login (n,p) ->
        match Authentication.authenticate n with
        | Ok principal -> 
            Authenticated (LoggedIn principal, (UICapability.Capabilities.mainCaps principal) ), Cmd.none
        | Error err -> 
            printfn ".. %A" err
            state, Cmd.none

    | Authenticated (a,c), SelectCustomer (principal, customerName) ->
        match Authentication.customerIdForName customerName with
            | Ok customerId -> 
                // found -- change state
                let state = Authenticated( CustomerSelected(principal, customerId), c )
                state, Cmd.none
            | Error err -> 
                // not found -- stay in originalState 
                printfn ".. %A" err
                state, Cmd.none
            
let getTodosUI getTodosCap principal dispatch =
    getTodosCap
    |> Option.map (fun _ -> 
        Html.button [
            prop.text "Get Todos"
            prop.onClick (fun _ -> GetTodos |> dispatch)
        ] 
    )
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
    | Authenticated ( (LoggedIn principal ), cap )  ->
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
    | Authenticated ( (CustomerSelected (principal, customerId )), cap ) -> //CustomerSelected (principal, customerId) ->
        // get the text for menu options based on capabilities that are present
        let menuOptionActions = 
            [
                getTodosUI cap principal dispatch
            ] 
            |> List.choose id
        Html.div [
            Html.h1 (sprintf "Logged in: %A" principal.Name )
            Html.h2 (sprintf "Customer: %A" customerId )
            Html.br []
            Html.div menuOptionActions
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

[<EntryPoint>]
ReactDOM.render(
    App(),
    document.getElementById "root"
)