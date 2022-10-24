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
    | CustomerSelected of User * CustomerId * string list option
type State = 
    | LoggedOut
    | Authenticated of Authenticated * option<FetchTodoCap>
        
let init() = LoggedOut, Cmd.none

let update msg state =
    match state with
    | Authenticated (a, capOpt) ->
        match msg with
        | GetTodos ->
            let cmd =
                match capOpt with
                | Some cap -> cap() //This is hard to read, because it hides the messages that will be called upon success or failure
                | None -> Cmd.none
            state, cmd
        | Login(_, _) -> failwith "Not Implemented"
        | GotTodos todos ->
            let principal, customerId =
                match a with
                | LoggedIn u -> failwith "Not Implemented"
                | CustomerSelected (principal, customerId, todos) -> principal, customerId
            let state = Authenticated( CustomerSelected(principal, customerId, Some todos), capOpt )
            state, Cmd.none

        | GotTodosError(_) -> failwith "Error getting To-Do's"
        | SelectCustomer(principal, customerName) ->
            match Authentication.customerIdForName customerName with
            | Ok customerId -> 
                // found -- change state
                let state = Authenticated( CustomerSelected(principal, customerId, None), capOpt )
                state, Cmd.none
            | Error err -> 
                // not found -- stay in originalState 
                printfn ".. %A" err
                state, Cmd.none
        | Logout -> failwith "Not Implemented"
            
    | LoggedOut ->
        match msg with
        | Login (u,p) ->
            match Authentication.authenticate u with
            | Ok principal ->
                let newState = Authenticated (
                    LoggedIn principal, 
                    (UICapability.Capabilities.mainCaps GotTodos GotTodosError principal) 
                )
                newState, Cmd.none
            | Error err -> 
                printfn ".. %A" err
                state, Cmd.none
        | _ -> state, Cmd.none
        // | GetTodos(_) -> failwith "Not Implemented"
        // | GotTodos(_) -> failwith "Not Implemented"
        // | GotTodosError(_) -> failwith "Not Implemented"
        // | SelectCustomer(_, _) -> failwith "Not Implemented"
        // | Logout -> failwith "Not Implemented"
        

    // match state, msg with
    // | _, Logout ->
    //     LoggedOut, Cmd.none
        
    // | Authenticated (a,c), GetTodos ->
    //     state,
    //     match c with
    //     | Some cap -> cap()
    //     | None -> Cmd.none

    // | _, GotTodos l ->
    //     console.log ("Got todos:", l)
    //     state, Cmd.none

    // | _, Login (n,p) ->
    //     match Authentication.authenticate n with
    //     | Ok principal -> 
    //         Authenticated (LoggedIn principal, (UICapability.Capabilities.mainCaps principal) ), Cmd.none
    //     | Error err -> 
    //         printfn ".. %A" err
    //         state, Cmd.none

    // | Authenticated (a,c), SelectCustomer (principal, customerName) ->
    //     match Authentication.customerIdForName customerName with
    //         | Ok customerId -> 
    //             // found -- change state
    //             let state = Authenticated( CustomerSelected(principal, customerId), c )
    //             state, Cmd.none
    //         | Error err -> 
    //             // not found -- stay in originalState 
    //             printfn ".. %A" err
    //             state, Cmd.none
            
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
    | Authenticated ( (CustomerSelected (principal, customerId, todos )), cap ) -> //CustomerSelected (principal, customerId) ->
        // get the text for menu options based on capabilities that are present
        let menuOptionActions = 
            [
                getTodosUI cap principal dispatch
            ] 
            |> List.choose id
        let todosUi = 
            match todos with
            | Some todos ->
                Html.ul (todos |> List.map (fun t -> Html.li [ Html.text t ]))
                
            | None -> null
        Html.div [
            Html.h1 (sprintf "Logged in: %A" principal.Name )
            Html.h2 (sprintf "Customer: %A" customerId )
            Html.br []
            Html.div menuOptionActions
            Html.div todosUi
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