module App

open Elmish
open Elmish.React
open Feliz
open Fable.Core

open TensGame.Model
open TensGame.View

let init () = NotStarted, Cmd.none

let removeIndex idx list =
    list
    |> Seq.mapi (fun i e -> (i <> idx, e))
    |> Seq.filter fst
    |> Seq.map snd
    |> List.ofSeq

module Extensions =
    [<Emit("setInterval($0, $1)")>]
    let setInterval (f: unit -> unit) (n: int) : int = jsNative

    [<Emit("clearInterval($0)")>]
    let clearInterval (n: int) : unit = jsNative

let random = System.Random()

let startTicking =
    Cmd.ofSub
        (fun dispatch ->
            let tickId =
                Extensions.setInterval (fun () -> let num = random.Next(1, 10) in dispatch (Tick num)) 999

            dispatch (Started tickId))

let stopTick tickId =
    Cmd.ofSub (fun dispatch -> Extensions.clearInterval tickId)

let update (msg: Msg) (state: State) : State * Cmd<_> =
    match msg, state with
    | Start, NotStarted
    | Start, Finished _ -> NotStarted, startTicking
    | Start _, _ -> failwith "Invalid state transition"

    | Started tickId, NotStarted -> (Running <| RunningState.Default tickId), Cmd.none
    | Started _, _ -> failwith "Invalid state transition"

    | Play (idx, num), Running state ->
        let total = List.sum state.prevClicks + num

        if total > 10 then
            Finished(Bust(num :: state.prevClicks, score = state.score)), stopTick state.tickId
        elif total = 10 then
            let scoreAdd =
                2.0
                ** ((List.length state.prevClicks - 1) |> float)
                |> int

            Running
                { state with
                      prevClicks = []
                      options = state.options |> removeIndex idx
                      score = state.score + scoreAdd },
            Cmd.none
        else
            Running
                { state with
                      prevClicks = num :: state.prevClicks
                      options = state.options |> removeIndex idx },
            Cmd.none

    | Play _, _ -> failwith "Invalid state transition"

    | Tick newEntry, Running state ->
        let numOptions = List.length state.options

        if numOptions > 10 then
            Finished(TimedOut state.score), stopTick state.tickId
        else
            Running
                { state with
                      options = state.options @ [ newEntry ] },
            Cmd.none

    | Tick _, _ -> failwith "Invalid state transition"


Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
|> Program.run
