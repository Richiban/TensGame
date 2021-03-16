module TensGame.Model

type RunningState =
    { prevClicks: int list
      options: int list
      score: int
      tickId: int }
    static member Default tickId =
        { prevClicks = []
          options = []
          score = 0
          tickId = tickId }

type EndState =
    | Bust of prevClicks: int list * score: int
    | TimedOut of score: int

type State =
    | NotStarted
    | Finished of EndState
    | Running of RunningState

type Msg =
    | Start
    | Started of tickId: int
    | Play of index: int * num: int
    | Tick of newEntry: int
