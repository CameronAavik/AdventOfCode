module Year2019Day13

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM
open CameronAavik.AdventOfCode.Y2019.Common

type GameData =
    { Ball : int64 * int64
      Paddle : int64 * int64
      BallDirection : int64
      Blocks : Set<int64 * int64>
      Score : int64 }

    static member create = { Ball = (-1L, -1L); Paddle = (-1L, -1L); Blocks = Set.empty; Score = 0L; BallDirection = 0L }

    static member addTile pos tile d =
        // update block counts
        let d = { d with Blocks = (if tile = 2L then Set.add else Set.remove) pos d.Blocks}

        // update ball or paddle if applicable
        let d = 
            match tile with
            | 3L -> { d with Paddle = pos }
            | 4L -> { d with Ball = pos; BallDirection = if fst d.Ball < fst pos then 1L else -1L }
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

let provideInput state = 
    if state.Blocks.IsEmpty then Seq.empty
    else
        let bx, by = state.Ball
        let px, py = state.Paddle
        let finalX = bx + (py - by - 1L) * state.BallDirection
        seq { if px > finalX then -1L
              elif px = finalX then 0L
              else 1L }

let gameIO = CallbackIO.create GameData.create GameData.processAllProgramOutput provideInput

let solvePart1 =
    bootProgram gameIO
    >> run
    >> getFromIOState (fun (_, g) -> g.Blocks.Count)
    
let solvePart2 = 
    bootProgram gameIO
    >> setVal (Position 0L) 2L
    >> run
    >> getFromIOState (fun (_, s) -> s.Score)

let solver = { parse = parseIntCodeFromFile; part1 = solvePart1; part2 = solvePart2 }