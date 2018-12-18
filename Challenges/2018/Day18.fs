module Year2018Day18

open CameronAavik.AdventOfCode.Common
open System

type Acre = Open | Trees | Lumberyard

let parseAcres lines =
    let charToAcre = function | '.' -> Open | '|' -> Trees | '#' -> Lumberyard | c -> failwithf "Invalid Char %c" c
    lines
    |> array2D
    |> Array2D.map charToAcre

let neighbours x y =
    [(x - 1, y - 1); (x, y - 1); (x + 1, y - 1); (x - 1, y); (x + 1, y); (x - 1, y + 1); (x, y + 1); (x + 1, y + 1)]

type NeighbourCounts = {openAcres: int; treeAcres: int; lumberyards: int}
let zeroCounts = {openAcres=0; treeAcres=0; lumberyards=0}
let addNeighbour counts neighbour = 
    match neighbour with
    | Open -> {counts with openAcres=counts.openAcres + 1}
    | Trees -> {counts with treeAcres=counts.treeAcres + 1}
    | Lumberyard -> {counts with lumberyards=counts.lumberyards + 1}

let step grid =
    let width = Array2D.length1 grid
    let height = Array2D.length2 grid
    let getNextState x y cur =
        let inBounds (x, y) = 0 <= x && x < width && 0 <= y && y < height
        let neighbourCounts =
            neighbours x y 
            |> List.filter inBounds 
            |> List.map (fun (x, y) -> grid.[x, y])
            |> List.fold addNeighbour zeroCounts
        match cur with
        | Open -> if neighbourCounts.treeAcres >= 3 then Trees else Open
        | Trees -> if neighbourCounts.lumberyards >= 3 then Lumberyard else Trees
        | Lumberyard -> if neighbourCounts.lumberyards = 0 || neighbourCounts.treeAcres = 0 then Open else Lumberyard
    Array2D.mapi getNextState grid

let array2DToSeq arr =
    seq { for r = 0 to Array2D.length1 arr - 1 do
            for c = 0 to Array2D.length2 arr - 1 do
              yield arr.[r, c] }

let score (grid : Acre [,]) =
    let counts = grid |> array2DToSeq |> Seq.fold addNeighbour zeroCounts
    counts.lumberyards * counts.treeAcres

let solvePart1 grid =
    Seq.init 10 id
    |> Seq.fold (fun g _ -> step g) grid
    |> score

let serialiseGrid =
    array2DToSeq
    >> Seq.map (function | Open -> '.' | Trees -> '|' | Lumberyard -> '#')
    >> String.Concat

let solvePart2 grid =
    let target = 1000000000
    let rec stepCached i grid cache =
        let serialised = serialiseGrid grid
        match Map.tryFind serialised cache with
        | Some x ->
            let cycleLength = i - x
            let stepsToTarget = (target - x) % cycleLength
            Seq.init stepsToTarget id 
            |> Seq.fold (fun g _ -> step g) grid
            |> score
        | None ->
            let cache' = Map.add serialised i cache
            let grid' = step grid
            stepCached (i + 1) grid' cache'
    stepCached 0 grid Map.empty

let solver = {parse = parseAcres; part1 = solvePart1; part2 = solvePart2}