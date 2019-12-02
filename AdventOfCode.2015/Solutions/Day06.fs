module Year2015Day06

open CameronAavik.AdventOfCode.Common

type Instr =
    | Toggle of int * int * int * int
    | TurnOn of int * int * int * int
    | TurnOff of int * int * int * int

let parseLine (line : string) = 
    let positions = extractInts line
    let (x1, y1, x2, y2) = positions.[0], positions.[1], positions.[2], positions.[3]
    let case =
        if line.StartsWith("toggle") then Toggle
        elif line.StartsWith("turn on") then TurnOn
        else TurnOff
    case (x1, y1, x2, y2)

let parse = parseEachLine parseLine

let solvePart1 input =
    let grid = 
        Array.init 1000 (fun _ -> Array.init 1000 (fun _ -> false))

    let applyRule grid instr =
        match instr with
        | Toggle (x1, y1, x2, y2) -> 
            grid
            |> Array.mapi (fun y row -> Array.mapi (fun x cell -> if x >= x1 && x <= x2 && y >= y1 && y <= y2 then not cell else cell) row)
        | TurnOn (x1, y1, x2, y2) -> 
            grid
            |> Array.mapi (fun y row -> Array.mapi (fun x cell -> if x >= x1 && x <= x2 && y >= y1 && y <= y2 then true else cell) row)
        | TurnOff (x1, y1, x2, y2) -> 
            grid
            |> Array.mapi (fun y row -> Array.mapi (fun x cell -> if x >= x1 && x <= x2 && y >= y1 && y <= y2 then false else cell) row)

    input
    |> Seq.fold applyRule grid
    |> Array.collect id
    |> Array.filter id
    |> Array.length

let solvePart2 input =
    let grid = 
        Array.init 1000 (fun _ -> Array.init 1000 (fun _ -> 0))

    let applyRule grid instr =
        match instr with
        | Toggle (x1, y1, x2, y2) -> 
            grid
            |> Array.mapi (fun y row -> Array.mapi (fun x cell -> if x >= x1 && x <= x2 && y >= y1 && y <= y2 then (cell + 2) else cell) row)
        | TurnOn (x1, y1, x2, y2) -> 
            grid
            |> Array.mapi (fun y row -> Array.mapi (fun x cell -> if x >= x1 && x <= x2 && y >= y1 && y <= y2 then (cell + 1) else cell) row)
        | TurnOff (x1, y1, x2, y2) -> 
            grid
            |> Array.mapi (fun y row -> Array.mapi (fun x cell -> if x >= x1 && x <= x2 && y >= y1 && y <= y2 then max 0 (cell - 1) else cell) row)

    input
    |> Seq.fold applyRule grid
    |> Array.collect id
    |> Array.sum

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }