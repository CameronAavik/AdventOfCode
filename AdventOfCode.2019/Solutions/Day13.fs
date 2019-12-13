module Year2019Day13

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM

type GameData =
    { Ball : int64 * int64
      Paddle : int64 * int64
      Blocks : Set<int64 * int64>
      Score : int64 }

    static member create = { Ball = (-1L, -1L); Paddle = (-1L, -1L); Blocks = Set.empty; Score = 0L }

    static member addTile pos tile d =
        // update block counts
        let d = { d with Blocks = (if tile = 2L then Set.add else Set.remove) pos d.Blocks}

        // update ball or paddle if applicable
        let d = 
            match tile with
            | 3L -> { d with Paddle = pos }
            | 4L -> { d with Ball = pos }
            | _ -> d

        d

    static member processProgramOutput a b c d =
        match a, b with
        | -1L, 0L -> { d with Score = c }
        | _ -> GameData.addTile (a, b) c d

    static member blocks d = d.Blocks

let readAllOutput data prog =
    let output, prog = prog |> runUntilInputRequired |> readAllOutput
    let data =
        output
        |> Seq.chunkBySize 3
        |> Seq.map (fun a -> a.[0], a.[1], a.[2])
        |> Seq.fold (fun d (a, b, c) -> GameData.processProgramOutput a b c d) data

    data, prog

let solvePart1 =
    bootProgram
    >> readAllOutput GameData.create
    >> fst
    >> GameData.blocks
    >> Set.count

let getMoveToMake (bx, by) (px, py) prevX =
    let dir = if prevX < bx then 1L else -1L
    let finalX = bx + (py - by - 1L) * dir

    if px > finalX then -1L
    elif px = finalX then 0L
    else 1L

let play prog =
    let rec play' prevX data prog =
        let data, prog = readAllOutput data prog
        if data.Blocks.IsEmpty then data.Score
        else
            prog
            |> writeToInput (getMoveToMake data.Ball data.Paddle prevX)
            |> play' (fst data.Ball) data
    play' 0L GameData.create prog
    

let solvePart2 = 
    bootProgram
    >> setVal (Position 0L) 2L
    >> play

let solver = { parse = parseIntCodeFromFile; part1 = solvePart1; part2 = solvePart2 }