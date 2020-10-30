module Year2019Day11

open AdventOfCode.FSharp.Common
open AdventOfCode.FSharp.Y2019.Common.Intcode

let turn (dx, dy) dir = if dir = 0L then (dy, -dx) else (-dy, dx)
let move (x, y) (dx, dy) = (x + dx, y + dy)

type GameState =
    { Grid : Map<int * int, int64>
      Dir : int * int
      Pos : int * int }

    static member create = { Grid = Map.empty; Pos = (0, 0); Dir = (0, -1) }
    static member paintPos color s = { s with Grid = Map.add s.Pos color s.Grid }
    static member turn turnDir s = { s with Dir = turn s.Dir turnDir }
    static member move s = { s with Pos = move s.Pos s.Dir }

let readOutput output =
    match Seq.toList output with
    | paint :: turnDir :: [] ->
        GameState.paintPos paint
        >> GameState.turn turnDir
        >> GameState.move
    | _ -> id

let writeInput state = Map.tryFind state.Pos state.Grid |> Option.defaultValue 0L

let printGrid grid =
    let updateBounds (x1, x2, y1, y2) (x, y) = (min x x1, max x x2, min y y1, max y y2)
    let minX, maxX, minY, maxY = Map.fold (fun bounds pos _ -> updateBounds bounds pos) (0, 0, 0, 0) grid

    seq {
        for y = minY to maxY do
            '\n'
            for x = minX to maxX do
                if (Map.tryFind (x, y) grid = Some 1L) then '█' else ' '
    } |> charsToStr

let rec runProgram state =
    function
    | Input f -> writeInput state |> f |> runProgram state
    | Output (o, s) -> runProgram (readOutput o state) s
    | Halted -> state

let solvePart1 =
    Computer.create
    >> run
    >> runProgram GameState.create
    >> (fun s -> s.Grid.Count)
    
let solvePart2 = 
    Computer.create
    >> run
    >> runProgram (GameState.create |> GameState.paintPos 1L)
    >> (fun s -> printGrid s.Grid)

let solver = { parse = parseIntCode; part1 = solvePart1; part2 = solvePart2 }