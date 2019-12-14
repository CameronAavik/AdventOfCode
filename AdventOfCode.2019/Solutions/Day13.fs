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

    static member processSingleProgramOutput a b c d =
        match a, b with
        | -1L, 0L -> { d with Score = c }
        | _ -> GameData.addTile (a, b) c d

    static member processAllProgramOutput output data =
        output
        |> Seq.chunkBySize 3
        |> Seq.map (fun a -> a.[0], a.[1], a.[2])
        |> Seq.fold (fun d (a, b, c) -> GameData.processSingleProgramOutput a b c d) data

    static member fromProgramOutput output = GameData.processAllProgramOutput output GameData.create

    static member blocks d = d.Blocks

let solvePart1 =
    bootProgram
    >> runUntilHalt
    >> readAllOutput
    >> fst
    >> GameData.fromProgramOutput
    >> GameData.blocks
    >> Set.count

let getMoveToMake (bx, by) (px, py) prevX =
    let dir = if prevX < bx then 1L else -1L
    let finalX = bx + (py - by - 1L) * dir

    if px > finalX then -1L
    elif px = finalX then 0L
    else 1L

let handleGameOutput output (prevX, data) =
    let data = GameData.processAllProgramOutput output data
    let input = 
        if data.Blocks.IsEmpty then None
        else Some (getMoveToMake data.Ball data.Paddle prevX)
    input, (fst data.Ball, data)
    
let solvePart2 = 
    bootProgram
    >> setVal (Position 0L) 2L
    >> setCallbackSingle handleGameOutput (0L, GameData.create)
    >> runUntilHalt
    >> getInputCallbackState
    >> (fun (_, s) -> s.Score)

let solver = { parse = parseIntCodeFromFile; part1 = solvePart1; part2 = solvePart2 }