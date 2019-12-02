module Year2015Day03

open CameronAavik.AdventOfCode.Common

type Dir = N | S | E | W
let parseDir = function | '^' -> N | 'v' -> S | '>' -> E | '<' -> W | c -> failwithf "Invalid: %A" c

let parse = parseFirstLine asString >> Seq.map parseDir

let solvePart1 input =
    input
    |> Seq.mapFold (fun (x, y) dir ->
        let newPos =
            match dir with
            | N -> (x, y - 1)
            | S -> (x, y + 1)
            | E -> (x + 1, y)
            | W -> (x - 1, y)
        newPos, newPos) (0, 0)
    |> fst
    |> Seq.append [(0, 0)]
    |> Seq.groupBy id
    |> Seq.length

let solvePart2 input =
    input
    |> Seq.mapFold (fun ((x1, y1), (x2, y2), isFirstRobot) dir ->
        let (x, y) = if isFirstRobot then (x1, y1) else (x2, y2)
        let newPos =
            match dir with
            | N -> (x, y - 1)
            | S -> (x, y + 1)
            | E -> (x + 1, y)
            | W -> (x - 1, y)
        let (x1, y1) = if isFirstRobot then newPos else (x1, y1)
        let (x2, y2) = if not isFirstRobot then newPos else (x2, y2)
        [(x1, y1); (x2, y2)], ((x1, y1), (x2, y2), not isFirstRobot)) ((0, 0), (0, 0), true)
    |> fst
    |> Seq.concat
    |> Seq.append [(0, 0)]
    |> Seq.groupBy id
    |> Seq.length

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }