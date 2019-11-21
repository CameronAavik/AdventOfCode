module Year2016Day22

open CameronAavik.AdventOfCode.Common
open System.IO
open System.Collections.Generic

type Node = { X : int; Y : int; Size : int; Used : int; Avail : int; UsePc : int }
let fromInts (ints : int []) = { X = ints.[0]; Y = ints.[1]; Size = ints.[2]; Used = ints.[3]; Avail = ints.[4]; UsePc = ints.[5]}

let parse (filename : string) =
    File.ReadLines filename
    |> Seq.skip 2
    |> Seq.map extractInts
    |> Seq.map fromInts
    |> Seq.toArray

let isAdjacent n1 n2 =
    let xDiff = n1.X - n2.X |> abs
    let yDiff = n1.Y - n2.Y |> abs
    (xDiff = 0 && yDiff = 1) || (xDiff = 1 && yDiff = 0)

let solvePart1 (lines : Node []) =
    Array.allPairs lines lines
    |> Array.filter (fun (n1, n2) -> n1.Used <> 0 && n1 <> n2 && n1.Used <= n2.Avail)
    |> Array.length

let solvePart2 lines =
    let minSize = lines |> Array.map (fun n -> n.Size) |> Array.min
    let maxX = lines |> Array.map (fun x -> x.X) |> Array.max
    let maxY = lines |> Array.map (fun x -> x.Y) |> Array.max

    let invalidSpots = lines |> Array.filter (fun n -> n.Used > minSize) |> Array.map (fun n -> n.X, n.Y) |> HashSet<_>

    let emptySpace = Array.find (fun n -> n.Used = 0) lines |> (fun n -> n.X, n.Y)
    let startingSpace = (maxX, 0)

    let seen = new HashSet<_>()
    let h ((x, y), (ex, ey)) =
        let endDist = x + y
        let emptyDist = abs (ex - x) + abs (ey - y) - 1
        endDist + emptyDist
    let isFinished ((x, y), _) = x = 0 && y = 0

    let getAllPossibleSwaps ((x, y), (ex, ey)) =
        let isValidSwap newEmpty = not (invalidSpots.Contains newEmpty)
        let applySwap newEmpty =
            if (x, y) = newEmpty then (ex, ey), newEmpty
            else (x, y), newEmpty

        seq {
            if ex > 0 then (ex - 1, ey)
            if ex < maxX then (ex + 1, ey)
            if ey > 0 then (ex, ey - 1)
            if ey < maxY then (ex, ey + 1) } 
            |> Seq.filter isValidSwap 
            |> Seq.map applySwap

    let rec astar fringe =
        if Set.isEmpty fringe then None
        else
            let (_, negDist, vertex) as minElem = Set.minElement fringe
            let fringe' = Set.remove minElem fringe
            let dist = -negDist
            if seen.Contains(vertex) then astar fringe'
            elif isFinished vertex then Some dist
            else
                seen.Add vertex |> ignore
                let swaps = getAllPossibleSwaps vertex
                swaps
                |> Seq.map (fun v -> (dist + 1, v))
                |> Seq.map (fun (dist, v) -> ((h v) + dist, -dist, v))
                |> Set.ofSeq
                |> Set.union fringe'
                |> astar
                
    let firstState = startingSpace, emptySpace
    match set [(0, 0, firstState)] |> astar with
    | Some x -> x
    | None -> failwith "Couldn't find a solution"

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }