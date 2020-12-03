module Year2015Day25

open AdventOfCode.FSharp.Common

let parse = parseFirstLine extractInts

let getCode row column =
    let n = row + column - 1
    let diagEnd = n * (n + 1) / 2
    let repetitions = diagEnd - row
    let rec genNext c n =
        if n = 0 then c
        else genNext ((c * 252533L) % 33554393L) (n - 1)
    genNext 20151125L repetitions

let solvePart1 (input : int []) =
    let row, column = input.[0], input.[1]
    getCode row column

let solver = { parse = parse; part1 = solvePart1; part2 = (fun _ -> "Advent of Code Finished!") }