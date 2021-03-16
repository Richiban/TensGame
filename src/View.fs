module TensGame.View

open Feliz
open Elmish
open Fable
open Fable.Core

open TensGame.Model

let layout (subtitle: string) contents =
    Html.div (
        [ Html.h1 [ Html.text "A Game of Tens" ]
          Html.div [ Html.text subtitle ] ]
        @ contents
    )

let render (state: State) (dispatch: Msg -> unit) =
    match state with
    | NotStarted ->
        layout
            "Click numbers as they appear, picking those that add up to ten. Don't go over ten or you're bust!"
            [ Html.p [ Html.button [ prop.text "Start game"
                                     prop.onClick (fun _ -> dispatch Start) ] ] ]

    | Finished state ->

        let subtitle =
            match state with
            | Bust (prevClicks, _) ->
                let prev =
                    prevClicks
                    |> Seq.map string
                    |> String.concat " + "

                let total = prevClicks |> Seq.sum

                sprintf "Game over! %s = %i" prev total
            | TimedOut _ -> "Game over! Out of time!"

        let score =
            match state with
            | Bust (_, score)
            | TimedOut score -> score

        layout
            subtitle
            [ Html.div [ Html.text (sprintf "Your score was: %i" score) ]
              Html.div [ Html.button [ prop.text "Play again"
                                       prop.onClick (fun _ -> dispatch Start) ] ] ]

    | Running state ->
        layout
            (string state.score)
            [ Html.div [ Html.div [ Html.text "You're playing!" ]
                         Html.div [] ]
              Html.div (
                  state.options
                  |> List.mapi
                      (fun i item ->
                          Html.button [ prop.text (item)
                                        prop.onClick (fun _ -> dispatch (Play(i, item))) ])
              ) ]
