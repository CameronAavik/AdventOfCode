module Year2015Day02

open CameronAavik.AdventOfCode.Common

type Prism = { W : int; H : int; L : int; }
let asPrism (arr : int []) =
    { W = arr.[0]; H = arr.[1]; L = arr.[2]}

let parse = parseEachLine extractInts >> Seq.map asPrism

let solvePart1 input =
    input
    |> Seq.sumBy (fun p -> 
        let sides = [| p.L * p.W; p.W * p.H; p.H * p.L |] 
        let area = 2 * (Seq.sum sides)
        area + (Seq.min sides))

let solvePart2 input =
    input
    |> Seq.sumBy (fun p ->
        let dims = [| p.L; p.W; p.H |]
        let wrap = 2 * (Seq.sum dims - Seq.max dims)
        let bow = p.L * p.W * p.H
        wrap + bow)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }