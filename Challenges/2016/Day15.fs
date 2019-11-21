module Year2016Day15

open CameronAavik.AdventOfCode.Common
open System

type Disc = { Number: int; Positions: int; Time: int; Position: int }

let asDisc line =
    let ints = extractInts line
    { Number = ints.[0]; Positions = ints.[1]; Time = ints.[2]; Position = ints.[3] }

let parse = parseEachLine asDisc >> Seq.toArray

let toRemainders discs =
    discs
    |> Array.map (fun d -> {d with Position = (d.Position + d.Number) % d.Positions})
    |> Array.map (fun d -> ((d.Positions - d.Position) % d.Positions, d.Positions))

let chineseRemainderTheorem remainders =
    let sorted = remainders |> Array.sortByDescending snd |> Array.toList
    let rec sieve remainders s =
        match remainders with
        | [] -> s |> Seq.tryHead
        | (a, n) :: xs -> s |> Seq.filter (fun x -> a = x % n) |> sieve xs

    let maxValue = Seq.map snd sorted |> Seq.reduce (*)
    sieve sorted [0 .. (maxValue - 1)]
       
let solvePart1 discs =
    let remainders = toRemainders discs
    (remainders |> chineseRemainderTheorem).Value

let solvePart2 discs = 
    let newDiscs = Array.append discs [| {Number = discs.Length + 1; Positions = 11; Time = 0; Position = 0 }|]
    solvePart1 newDiscs

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }