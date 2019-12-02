module Year2015Day10

open CameronAavik.AdventOfCode.Common

let toCounts str =
    str |> Seq.map (fun c -> (1, int c - int '0')) |> Seq.toList

let rec compress acc counts =
    match counts with
    | [] -> acc []
    | (n1, c1) :: (n2, c2) :: xs when c1 = c2 -> compress acc ((n1 + n2, c1) :: xs)
    | x :: xs -> compress (fun a -> acc (x :: a)) xs

let iterate counts =
    counts
    |> compress id
    |> List.map (fun (n, c) -> [ (1, n); (1, c) ])
    |> List.collect id

let parse = parseFirstLine toCounts

let solvePart1 input =
    let rec iterN n counts =
        if n = 0 then counts |> List.sumBy fst
        else iterate counts |> iterN (n - 1)
    iterN 40 input

let solvePart2 input =
    let rec iterN n counts =
        if n = 0 then counts |> List.sumBy fst
        else iterate counts |> iterN (n - 1)
    iterN 50 input

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }