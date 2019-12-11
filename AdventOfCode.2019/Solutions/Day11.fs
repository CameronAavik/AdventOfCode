module Year2019Day11

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM

let turn (dx, dy) dir = if dir = 0L then (dy, -dx) else (-dy, dx)
let move (x, y) (dx, dy) = (x + dx, y + dy)

let rec run grid pos dir prog =
    let paintToWrite = Map.tryFind pos grid |> Option.defaultValue 0L
    let prog = prog |> writeToInput paintToWrite |> runUntilOutput

    if prog.IsHalted then grid
    else
        let paint, prog = readFromOutput prog
        let turnDir, prog = prog |> runUntilOutput |> readFromOutput

        let newDir = turn dir turnDir
        run (Map.add pos paint grid) (move pos newDir) newDir prog

let printGrid grid =
    let updateBounds (x1, x2, y1, y2) (x, y) = (min x x1, max x x2, min y y1, max y y2)
    let minX, maxX, minY, maxY = Map.fold (fun bounds pos _ -> updateBounds bounds pos) (0, 0, 0, 0) grid

    seq {
        for y = minY to maxY do
            '\n'
            for x = minX to maxX do
                if (Map.tryFind (x, y) grid = Some 1L) then '█' else ' '
    } |> charsToStr

let solvePart1 =
    bootProgram
    >> run Map.empty (0, 0) (0, -1)
    >> Map.count
    
let solvePart2 = 
    bootProgram
    >> run (Map.ofList [(0, 0), 1L]) (0, 0) (0, -1)
    >> printGrid

let solver = { parse = parseIntCodeFromFile; part1 = solvePart1; part2 = solvePart2 }