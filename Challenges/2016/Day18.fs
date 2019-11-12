module Year2016Day18

open CameronAavik.AdventOfCode.Common

let parse = parseFirstLine asString >> Seq.map (function | '^' -> true | _ -> false) >> Seq.toArray

let isTrap l c r =
    (l && c && (not r)) ||
    (c && r && (not l)) ||
    (l && (not c) && (not r)) ||
    (r && (not c) && (not l))

let nextState (row : bool []) =
    let nextRow =
        row
        |> Array.windowed 3
        |> Array.map (fun w -> isTrap w.[0] w.[1] w.[2])
    Array.concat [| [| row.[1] |]; nextRow; [| row.[row.Length - 2] |] |]

let solve input rows =
    Seq.init rows id
    |> Seq.mapFold (fun row _ -> row, nextState row) input
    |> fst
    |> Seq.collect id
    |> Seq.where not
    |> Seq.length

let solvePart1 input = solve input 40
let solvePart2 input = solve input 400000

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }