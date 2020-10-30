module Year2017Day17

open AdventOfCode.FSharp.Common

let getInsertPositions i skip = List.fold (fun l n -> (((List.head l) + skip) % n + 1) :: l) [0] (List.init i ((+)1))
let rec findTarget target = function
    | [] -> 0
    | x :: xs when x = target -> List.length xs
    | x :: xs -> findTarget (target + if x < target then - 1 else 0) xs 
let part1 = getInsertPositions 2017 >> (fun pos -> findTarget ((List.head pos) + 1) pos)

let rec part2 afterZero i n skip = 
    if n = 50000001 then afterZero 
    else (i + skip) % n |> (fun next -> part2 (if next = 0 then n else afterZero) (next + 1) (n + 1) skip)
let solver = {parse = parseFirstLine asInt; part1 = part1; part2 = part2 0 0 1}