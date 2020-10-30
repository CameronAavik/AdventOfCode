module Year2018Day06

open AdventOfCode.FSharp.Common
open System

let asPoint = splitBy ", " (fun x -> (int x.[0], int x.[1]))

let manhattan x y (px, py) = abs (px-x) + abs (py-y)
let getClosestNode (pts : (int * int) array) x y =
    let updateMin (currentMin, nodes) ptIndex =
        let pt = pts.[ptIndex]
        let dist = manhattan x y pt
        if   dist < currentMin then (dist, ptIndex::[])
        elif dist = currentMin then (currentMin, ptIndex::nodes)
        else                        (currentMin, nodes)
    let numPoints = Array.length pts
    Seq.init numPoints id |> Seq.fold updateMin (Int32.MaxValue, []) |> snd

let getRanges =
    let updateRanges (minX, maxX, minY, maxY) (x, y) =
        min x minX, max x maxX, min y minY, max y maxY
    Seq.fold updateRanges (Int32.MaxValue, Int32.MinValue, Int32.MaxValue, Int32.MinValue)
        
let row i (arr: 'T[,]) = arr.[i..i, *] |> Seq.cast<'T> |> Seq.toArray
let col i (arr: 'T[,]) = arr.[*, i..i] |> Seq.cast<'T> |> Seq.toArray
let solvePart1 pts =
    let minX, maxX, minY, maxY = getRanges pts
    let width, height = maxX - minX, maxY - minY
    let offsetPts = pts |> Seq.map (fun (x, y) -> (x - minX, y - minY)) |> Seq.toArray
    let grid = Array2D.init (height + 1) (width + 1) (getClosestNode offsetPts)
    let borders = Array.concat [row 0 grid; row height grid; col 0 grid; col width grid] |> Seq.concat
    let excluded = Set.ofSeq borders
    grid
    |> Seq.cast<int list>
    |> Seq.filter (fun dups -> List.length dups = 1)
    |> Seq.map List.head
    |> Seq.countBy id
    |> Seq.filter (fst >> (fun pt -> Set.contains pt excluded) >> not)
    |> Seq.maxBy snd
    |> snd

let getDists1D pts =
    let rec step l r lc rc i dists pts =
        match pts with
        | [] -> dists
        | x :: xs when x = i -> step l r (lc + 1) (rc - 1) i dists xs
        | _ -> step (l + lc) (r - rc) lc rc (i + 1) ((l+r) :: dists) pts
    let sorted = Seq.sort pts |> Seq.toList
    let leftmost = List.head sorted
    let total = List.sumBy (fun pt -> pt - leftmost) sorted
    step 0 total 0 (List.length sorted) leftmost [] sorted

let solvePart2 pts =
    let xDists = Seq.map fst pts |> getDists1D |> List.sort
    let yDists = Seq.map snd pts |> getDists1D |> List.sort
    xDists
    |> List.collect (fun x -> yDists |> List.takeWhile (fun y -> x + y < 10000))
    |> List.length

let solver = {parse = parseEachLine asPoint; part1 = solvePart1; part2 = solvePart2}