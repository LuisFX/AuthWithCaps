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

type PageCaps = {
    TodosCap1: FetchTodoCap option
    TodosCap2: FetchTodoCap option
}

type Msg =
    | Login of string * string
    | GetTodos
    | GotTodos of Todo list // this would actually be when server
    | GotTodosError of exn
    | SelectUser of UserPrincipal * string
    | DeselectUser
    | Logout

type Authenticated2 =
    | LoggedIn of UserPrincipal
    | UserSelected of UserPrincipal * UserId * Todo list option

type State =
    | LoggedOut
    | Authenticated of Authenticated2 * PageCaps option

let init() = LoggedOut, Cmd.none

let loginCmd state u =
    match Authentication.authenticate u with
    | Ok principal ->
        let newState = Authenticated (
            LoggedIn principal,
            None
        )
        newState, Cmd.none
    | Error err ->
        printfn ".. %A" err
        state, Cmd.none

let update msg state =
    match state with
    | LoggedOut ->
        match msg with
        | Login (u,p) ->
           loginCmd state u
        | _ -> state, Cmd.none
    | Authenticated( LoggedIn (p), pageCaps) ->
        match msg with
        | SelectUser(principal, userName) ->
            match Authentication.userIdForName userName with
            | Ok userId ->
                // found -- change state
                let pageCaps = (UICapability.Capabilities.mainPageCaps userId principal)
                let getTodosWired1 = pageCaps.getTodos1
                let getTodosWired2 = pageCaps.getTodos2

                let pageCapsOpts = Some { TodosCap1 = getTodosWired1; TodosCap2 = getTodosWired1 }
                let newState:State = Authenticated( UserSelected( principal, userId, None ), pageCapsOpts )

                newState, Cmd.none
            | Error err ->
                // not found -- stay in originalState
                printfn ".. %A" err
                state, Cmd.none
    | Authenticated(Item1 = UserSelected(principal, userId, todos); Item2 = pageCapsOpt;) ->
        match msg with
        | DeselectUser ->
            Authenticated (LoggedIn(principal), pageCapsOpt), Cmd.none
        | GetTodos ->
            let cmd =
                match pageCapsOpt with
                | Some pageCaps ->
                    match pageCaps.TodosCap1 with
                    | Some cap ->
                        Cmd.OfAsyncWith.either Async.StartImmediate cap () GotTodos GotTodosError
                    | None -> Cmd.none
                | None -> Cmd.none
            state, cmd
        | GotTodos todos ->
            let state = Authenticated (UserSelected(principal, userId, Some todos), None)
            state, Cmd.none

        | GotTodosError(_) -> failwith "Error getting To-Do's"
        | Logout -> failwith "Not Implemented"
        | Login(_, _) -> failwith "Not Implemented"

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

            Html.span "Pick a user"
            Html.br []
            Html.button [
                prop.text "Luis"
                prop.onClick (fun _ -> SelectUser (principal, "luis") |> dispatch)
            ]
            Html.br []
            Html.button [
                prop.text "maxime"
                prop.onClick (fun _ ->  SelectUser (principal, "maxime") |> dispatch)
            ]
            Html.br []
            Html.br []
            Html.button [
                prop.text "Logout"
                prop.onClick (fun _ -> Logout |> dispatch)
            ]
        ]
    | Authenticated ( (UserSelected (principal, customerId, todos )), pageCapsOpt ) -> //CustomerSelected (principal, customerId) ->
        // get the text for menu options based on capabilities that are present
        let menuOptionActions =
            [
                match pageCapsOpt with
                | Some pageCaps ->
                    getTodosUI pageCaps.TodosCap1 principal dispatch
                | None -> None
            ]
            |> List.choose id
        let todosUi =
            match todos with
            | Some todos -> Html.ul (todos |> List.map (fun t -> Html.li [ Html.text t.Description; Html.span "=" ; Html.text (t.Completed.ToString()) ]))
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
                prop.onClick (fun _ -> DeselectUser |> dispatch)
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