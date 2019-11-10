module Year2018Day18

open CameronAavik.AdventOfCode.Common
open System.IO

type Acre = Open | Trees | Lumberyard
let charToAcre = 
    function
    | '.' -> Open
    | '|' -> Trees
    | '#' -> Lumberyard
    | c -> failwithf "Invalid Char %c" c

let parseAcres = array2D >> Array2D.map charToAcre

let neighbours x y =
    [|(x - 1, y - 1); (x, y - 1); (x + 1, y - 1); (x - 1, y); 
      (x + 1, y); (x - 1, y + 1); (x, y + 1); (x + 1, y + 1)|]

type NeighbourCounts = {openAcres: int; treeAcres: int; lumberyards: int}
let zeroCounts = {openAcres=0; treeAcres=0; lumberyards=0}
let addNeighbourToCounts counts = 
    function
    | Open -> {counts with openAcres=counts.openAcres + 1}
    | Trees -> {counts with treeAcres=counts.treeAcres + 1}
    | Lumberyard -> {counts with lumberyards=counts.lumberyards + 1}

let getNextCellState cur {treeAcres=trees; lumberyards=lumberyards} =
    match cur with
    | Open -> if trees >= 3 then Trees else Open
    | Trees -> if lumberyards >= 3 then Lumberyard else Trees
    | Lumberyard -> if lumberyards = 0 || trees = 0 then Open else Lumberyard

let step grid =
    let width = Array2D.length1 grid
    let height = Array2D.length2 grid
    let inBounds (x, y) = 0 <= x && x < width && 0 <= y && y < height
    let getNextState x y cur =
        neighbours x y 
        |> Array.filter inBounds 
        |> Array.map (fun (x, y) -> grid.[x, y])
        |> Array.fold addNeighbourToCounts zeroCounts
        |> getNextCellState cur
    Array2D.mapi getNextState grid

let score grid =
    let counts = grid |> Seq.cast<Acre> |> Seq.fold addNeighbourToCounts zeroCounts
    counts.lumberyards * counts.treeAcres

let stepN n grid = Seq.init n id |> Seq.fold (fun g _ -> step g) grid
let solvePart1 = stepN 10 >> score
let solvePart2 grid =
    let rec stepCached i grid cache =
        match Map.tryFind grid cache with
        | Some x ->
            let cycleLength = i - x
            let stepsToTarget = (1000000000 - x) % cycleLength
            grid |> stepN stepsToTarget |> score
        | None -> stepCached (i + 1) (step grid) (Map.add grid i cache)
    stepCached 0 grid Map.empty

let solver = {parse = File.ReadLines >> parseAcres; part1 = solvePart1; part2 = solvePart2}