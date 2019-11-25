module Year2015Day17

open CameronAavik.AdventOfCode.Common

let parse = parseEachLine asInt

let solvePart1 input =
    let sizes = input |> Seq.sortByDescending id |> Seq.toArray
    let firstRow = Array.init 151 (fun i -> if i = 0 then 1 else 0)
    let nextRow (size : int) (row : int []) =
        let nextCell cur_sum =
            if cur_sum < size then row.[cur_sum]
            else row.[cur_sum] + row.[cur_sum - size]
        [| 0 .. 150 |] |> Array.map nextCell
    let arr = Array.foldBack nextRow sizes firstRow
    arr.[150]

let solvePart2 input =
    let sizes = input |> Seq.sortByDescending id |> Seq.toArray
    let firstRow = Array.init 151 (fun i -> if i = 0 then (0, 1) else (sizes.Length + 1, 0))
    let nextRow (size : int) (row : (int * int) []) =
        let nextCell cur_sum =
            if cur_sum < size then row.[cur_sum]
            else
                let c1, w1 = row.[cur_sum]
                let c2, w2 = row.[cur_sum - size]
                if c1 < c2 + 1 then
                    c1, w1
                elif c1 > c2 + 1 then
                    c2 + 1, w2
                else
                    c1, w1 + w2

        [| 0 .. 150 |] |> Array.map nextCell
    let arr = Array.foldBack nextRow sizes firstRow
    arr.[150] |> snd

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }