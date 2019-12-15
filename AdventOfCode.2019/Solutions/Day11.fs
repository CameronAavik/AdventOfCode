module Year2019Day11

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM

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

let writeInput state =
    seq { Map.tryFind state.Pos state.Grid |> Option.defaultValue 0L }

let printGrid grid =
    let updateBounds (x1, x2, y1, y2) (x, y) = (min x x1, max x x2, min y y1, max y y2)
    let minX, maxX, minY, maxY = Map.fold (fun bounds pos _ -> updateBounds bounds pos) (0, 0, 0, 0) grid

    seq {
        for y = minY to maxY do
            '\n'
            for x = minX to maxX do
                if (Map.tryFind (x, y) grid = Some 1L) then '█' else ' '
    } |> charsToStr

let programIO = CallbackIO.create GameState.create readOutput writeInput

let solvePart1 =
    bootProgram programIO
    >> run
    >> getFromIOState (fun (_, s) -> s.Grid.Count)
    
let solvePart2 = 
    bootProgram programIO
    >> mapIO (fun (q, s) -> q, GameState.paintPos 1L s)
    >> run
    >> getFromIOState (fun (_, s) -> s.Grid)
    >> printGrid

let solver = { parse = parseIntCodeFromFile; part1 = solvePart1; part2 = solvePart2 }