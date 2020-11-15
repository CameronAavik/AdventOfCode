module Year2019Day15

open AdventOfCode.FSharp.Common
open AdventOfCode.FSharp.Y2019.Common.Intcode

let provideInput systemId =
    function
    | Input f -> f systemId
    | _ -> failwith "Expected an input"

let readOutput =
    function
    | Output (o, _) -> o
    | _ -> failwith "Expected an output"

[<NoComparison>]
[<NoEquality>]
type Robot =
    { Pos : int * int
      IsOxygen : bool
      Program : ProgramState }

    static member create intcode = { Pos = (0, 0); Program = Computer.create intcode |> run; IsOxygen = false }

let move (x, y) dir =
    if dir = 1 then (x, y - 1)
    elif dir = 2 then (x, y + 1)
    elif dir = 3 then (x - 1, y)
    else (x + 1, y)

let tryMoveRobotInDir r dir =
    match r.Program with
    | Input f ->
        match f (int64 dir) with
        | Output ([response], p) ->
            if response = 0L then None
            else Some { Pos = move r.Pos dir; IsOxygen = response = 2L; Program = p }
        | _ -> failwith "Program should have produced a single output"
    | _ -> failwith "Program should have been expecting input"

let getEdges r = [|1 .. 4|] |> Array.choose (tryMoveRobotInDir r)

let bfs stopCondition initRobot =
    let rec bfs' level seen steps =
        let level = level |> Array.filter (fun r -> Set.contains r.Pos seen |> not)
        if Array.isEmpty level then None, steps - 1
        else
            match Array.tryFind stopCondition level with
            | Some r -> Some r, steps
            | None ->
                let newSeen = Set.union seen (level |> Array.map (fun r -> r.Pos) |> Set.ofArray)
                let nextLevel = level |> Array.collect getEdges
                bfs' nextLevel newSeen (steps + 1)
    bfs' [| initRobot |] Set.empty 0

let solvePart1 =
    Robot.create
    >> bfs (fun r -> r.IsOxygen)
    >> snd

let solvePart2 =
    Robot.create
    >> bfs (fun r -> r.IsOxygen)
    >> fst
    >> Option.get
    >> bfs (fun _ -> false)
    >> snd

let solver = { parse = parseIntCode; part1 = solvePart1; part2 = solvePart2 }
