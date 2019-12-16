module Year2019Day15

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common.Intcode

let provideInput systemId =
    function
    | Input f -> f systemId
    | _ -> failwith "Expected an input"

let readOutput =
    function
    | Output (o, _) -> o
    | _ -> failwith "Expected an output"

type Robot =
    { Pos : int * int
      IsOxygen : bool
      Program : ProgramState }

let move (x, y) dir =
    if dir = 1 then (x, y - 1)
    elif dir = 2 then (x, y + 1)
    elif dir = 3 then (x - 1, y)
    else (x + 1, y)

// I will clean this up later...
let bfs initRobot stopCondition =
    let rec bfs' level seen steps =
        let level = level |> Array.filter (fun r -> Set.contains r.Pos seen |> not)
        if Array.isEmpty level then None, steps - 1
        else
            match Array.tryFind stopCondition level with
            | Some r -> Some r, steps
            | None ->
                let newSeen = Set.union seen (level |> Array.map (fun r -> r.Pos) |> Set.ofArray)
                let nextLevel =
                    level
                    |> Array.collect (fun r ->
                        [|1 .. 4|]
                        |> Array.choose (fun dir ->
                            match r.Program with
                            | Input f ->
                                match f (int64 dir) with
                                | Output ([response], p) ->
                                    if response = 0L then None
                                    else Some { Pos = move r.Pos dir; IsOxygen = response = 2L; Program = p }
                                | p -> failwith "Program should have produced a single output"
                            | _ -> failwith "Program should have been expecting input"
                        ))
                bfs' nextLevel newSeen (steps + 1)
    bfs' [| initRobot |] Set.empty 0

let solvePart1 intcode =
    let firstRobot = { Pos = (0, 0); Program = Computer.create intcode |> run; IsOxygen = false }
    bfs firstRobot (fun r -> r.IsOxygen) |> snd

let solvePart2 intcode =
    let firstRobot = { Pos = (0, 0); Program = Computer.create intcode |> run; IsOxygen = false }
    let destRobot, _ = bfs firstRobot (fun r -> r.IsOxygen)
    bfs (Option.get destRobot) (fun _ -> false) |> snd

let solver = { parse = parseIntCode; part1 = solvePart1; part2 = solvePart2 }