module Year2019Day13

open AdventOfCode.FSharp.Common
open AdventOfCode.FSharp.Y2019.Common.Intcode

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
    if state.Blocks.IsEmpty then None
    else
        let bx, by = state.Ball
        let px, py = state.Paddle
        let finalX = bx + (py - by - 1L) * state.BallDirection
        if px > finalX then Some -1L
            elif px = finalX then Some 0L
            else Some 1L

let rec runProgram state =
    function
    | Input f -> 
        match provideInput state with
        | Some i -> f i |> runProgram state
        | None -> state
    | Output (o, s) -> runProgram (GameData.processAllProgramOutput o state) s
    | Halted -> state

let solvePart1 =
    Computer.create
    >> run
    >> runProgram (GameData.create)
    >> (fun g -> g.Blocks.Count)
    
let solvePart2 = 
    Computer.create
    >> Computer.set 0 2L
    >> run
    >> runProgram (GameData.create)
    >> (fun s -> s.Score)

let solver = { parse = parseIntCode; part1 = solvePart1; part2 = solvePart2 }