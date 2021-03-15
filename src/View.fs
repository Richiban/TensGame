module TensGame.View

open Feliz
open Elmish
open Fable
open Fable.Core

open TensGame.Model

let layout (subtitle: string) contents =
    Html.div (
        [ 
          Html.h1 [ Html.text "A Game of Tens" ]
          Html.div [ Html.text subtitle ] ]
        @ contents
    )

let render (state: State) (dispatch: Msg -> unit) =
    match state with
    | NotStarted ->
        layout
            ""
            [ Html.div [ Html.button [ prop.text "Start game"
                                       prop.onClick (fun _ -> dispatch Start) ] ] ]

    | Finished (score, lastAttempt) ->
        layout
            (sprintf "Game over! (%i)" lastAttempt)
            [ Html.div [ Html.text (sprintf "Your score was: %i" score) ]
              Html.div [ Html.button [ prop.text "Play again"
                                       prop.onClick (fun _ -> dispatch Start) ] ] ]

    | Running state ->
        layout
            ""
            [ Html.div [ Html.div [ Html.text "You're playing!" ]
                         Html.div [ Html.text (List.sum state.prevClicks) ] ]
              Html.div (
                  state.options
                  |> List.mapi
                      (fun i item ->
                          Html.button [ prop.text (item)
                                        prop.onClick (fun _ -> dispatch (Play(i, item))) ])
              ) ]
