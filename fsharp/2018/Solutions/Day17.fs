module Year2018Day17

open AdventOfCode.FSharp.Common

let asScan (str : string) =
    let isX = str.[0] = 'x'
    let single, range = splitBy ", " (fun p -> int p.[0].[2..], p.[1]) str
    let range1, range2 = splitBy ".." (fun p -> int p.[0], int p.[1]) range.[2..]
    if isX then
        (single, range1), (single, range2)
    else
        (range1, single), (range2, single)

let getYBounds =
    let updateBounds (minY, maxY) (y1, y2) = min minY y1, max maxY y2
    Seq.map (fun ((_, y1), (_, y2)) -> (y1, y2))
    >> Seq.reduce updateBounds

type Tile = Clay | Sand | StillWater | MovingWater

let search x y dir grid sources =
    let rec search' x =
        let current = Map.tryFind (x, y) grid
        let below = Map.tryFind (x, y + 1) grid
        match (current, below) with
        | (Some Clay, _) -> x - dir, false, sources
        | (_, None) | (_, Some Sand) -> x, true, (x, y) :: sources
        | (Some MovingWater, Some MovingWater) -> x, true, sources
        | _ -> search' (x + dir)
    search' x

let processWaterSource maxY grid (x, y) =
    match Map.tryFind (x, y) grid with
    | Some MovingWater ->
        let rec processYPos grid sources y =
            match Map.tryFind (x, y) grid with
            | _ when (y > maxY) -> grid, sources
            | Some MovingWater -> grid, sources
            | Some Clay | Some StillWater ->
                let prevY = y - 1
                let left, leftOverflow, sources = search x prevY (-1) grid sources
                let right, rightOverflow, sources = search x prevY 1 grid sources
                let isOverflow = leftOverflow || rightOverflow
                let tileType = if isOverflow then MovingWater else StillWater
                let grid = 
                    seq { for x = left to right do yield ((x, prevY), tileType)}
                    |> Seq.fold (fun grid (k, v) -> Map.add k v grid) grid
                processYPos grid sources prevY
            | Some Sand | None ->
                processYPos (grid |> Map.add (x, y) MovingWater) sources (y + 1)
        processYPos grid [] (y + 1)
    | _ -> grid, []

let simulate grid bounds initialSource =
    let rec simulate' grid sources =
        match sources with
        | [] -> grid
        | src :: srcs ->
            let grid', newSources = processWaterSource bounds grid src
            let newSources' = List.fold (fun xs x -> x :: xs) srcs newSources
            simulate' grid' newSources'
    simulate' (grid |> Map.add initialSource MovingWater) [initialSource]

let getCounts scans =
    let minY, maxY = getYBounds scans
    let grid = seq {
        for (x1, y1), (x2, y2) in scans do
            for x = x1 to x2 do
                for y = y1 to y2 do
                    yield (x, y), Clay } |> Map.ofSeq
    simulate grid maxY (500, 0)
    |> Map.toSeq
    |> Seq.filter (fun ((_, y), _) -> minY <= y && y <= maxY)
    |> Seq.countBy snd
    |> Map.ofSeq

let solvePart1 = getCounts >> (fun counts -> Map.find StillWater counts + Map.find MovingWater counts)
let solvePart2 = getCounts >> Map.find StillWater

let solver = {parse = parseEachLine asScan; part1 = solvePart1; part2 = solvePart2}