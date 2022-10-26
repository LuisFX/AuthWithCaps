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

type Authenticated =
    | LoggedIn of UserPrincipal
    | UserSelected of UserPrincipal * UserId * string list option

type State<'PageCapsOpts> =
    | LoggedOut
    | Authenticated of Authenticated * 'PageCapsOpts

let init() = LoggedOut, Cmd.none

let update msg state =
    match state with
    | LoggedOut ->
        match msg with
        | Login (u,p) ->
            match Authentication.authenticate u with
            | Ok principal ->
                let pageCaps = (UICapability.Capabilities.mainPageCaps principal)
                let getTodosWired1 = pageCaps.getTodos1 GotTodos GotTodosError
                let getTodosWired2 = pageCaps.getTodos2

                let pageCapsOpts = {| getTodos1 = getTodosWired1; getTodos2 = getTodosWired2 |}

                let newState = Authenticated (
                    LoggedIn principal,
                    pageCapsOpts
                )
                newState, Cmd.none
            | Error err ->
                printfn ".. %A" err
                state, Cmd.none
        | _ -> state, Cmd.none
    | Authenticated (a, pageCaps) ->
        match msg with
        | GetTodos ->
            let cmd =
                match pageCaps.getTodos1 with
                | Some cap -> cap()  //This is hard to read, because it hides the messages that will be called upon success or failure
                | None -> Cmd.none
            state, cmd
        | Login(_, _) -> failwith "Not Implemented"
        | GotTodos todos ->
            let principal, userId =
                match a with
                | LoggedIn u -> failwith "Not Implemented"
                | UserSelected (principal, userId, todos) -> principal, userId
            let state = Authenticated( UserSelected(principal, userId, Some todos), pageCaps )
            state, Cmd.none

        | GotTodosError(_) -> failwith "Error getting To-Do's"
        | SelectUser(principal, userName) ->
            match Authentication.userIdForName userName with
            | Ok userId ->
                // found -- change state
                let state = Authenticated( UserSelected(principal, userId, None), pageCaps )
                state, Cmd.none
            | Error err ->
                // not found -- stay in originalState
                printfn ".. %A" err
                state, Cmd.none
        | Logout -> failwith "Not Implemented"

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
    | Authenticated ( (UserSelected (principal, customerId, todos )), pageCaps ) -> //CustomerSelected (principal, customerId) ->
        // get the text for menu options based on capabilities that are present
        let menuOptionActions =
            [
                getTodosUI pageCaps.getTodos1 principal dispatch
            ]
            |> List.choose id
        let todosUi =
            match todos with
            | Some todos -> Html.ul (todos |> List.map (fun t -> Html.li [ Html.text t ]))
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