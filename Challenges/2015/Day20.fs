module Year2015Day20

open CameronAavik.AdventOfCode.Common

let parse = parseFirstLine asInt

let rec getPrimeFactors n =
    let fac =
        [2 .. int (ceil (sqrt (float n)))]
        |> List.tryFind (fun i -> n % i = 0 && i <> n)

    match fac with
    | Some f -> f :: (getPrimeFactors (n / f))
    | None -> [n]

let rec allMultiples factors =
    match factors with
    | x :: xs -> 
        let ms = allMultiples xs
        let ms2 = ms |> Set.map (fun y -> y * x)
        Set.union ms ms2
    | [] -> Set.empty |> Set.add 1

let allFactors n = getPrimeFactors n |> allMultiples |> Set.add n

let solvePart1 input =
    Seq.initInfinite id
    |> Seq.find (fun i ->
        let mults = allFactors i
        let s = Seq.sum mults
        s * 10 >= input)

let solvePart2 input =
    Seq.initInfinite id
    |> Seq.find (fun i ->
        let mults = allFactors i
        let s = mults |> Seq.filter (fun m -> m = 0 || i / m <= 50) |> Seq.sum
        s * 11 >= input)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }