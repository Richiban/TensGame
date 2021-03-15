module TensGame.Model

type RunningState =
    { prevClicks: int list
      options: int list
      score: int
      tickId: int }
    static member Default tickId =
        { prevClicks = []
          options = [ 1; 5; 7; 9 ]
          score = 0
          tickId = tickId }

type State =
    | NotStarted
    | Finished of score: int * lastAttempt: int
    | Running of RunningState

type Msg =
    | Start
    | Started of tickId: int
    | Play of index: int * num: int
    | Tick of newEntry: int
