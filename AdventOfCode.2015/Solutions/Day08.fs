module Year2015Day08

open CameronAavik.AdventOfCode.Common

let parse = parseEachLine asString

let getStrLength str =
    let l = String.length str
    let rec parsePosition (count : int) i =
        if i >= l then count
        else
            let isLast = i = l - 1
            match str.[i] with
            | '\\' ->
                if isLast then count + 1
                else
                match str.[i + 1] with
                | '\\' -> parsePosition (count + 1) (i + 2)
                | 'x' when i <= l - 4 -> parsePosition (count + 1) (i + 4)
                | '"' -> parsePosition (count + 1) (i + 2)
                | _ -> parsePosition (count + 1) (i + 1)
            | '"' -> parsePosition count (i + 1)
            | _ -> parsePosition (count + 1) (i + 1)

    parsePosition 0 0

let encodeString str =
    let l = String.length str
    let rec parsePosition (count : int) i =
        if i >= l then count
        else
            let isLast = i = l - 1
            match str.[i] with
            | '\\' ->
                if isLast then count + 1
                else
                match str.[i + 1] with
                | '\\' -> parsePosition (count + 4) (i + 2)
                | 'x' when i <= l - 4 -> parsePosition (count + 5) (i + 4)
                | '"' -> parsePosition (count + 4) (i + 2)
                | _ -> parsePosition (count + 1) (i + 1)
            | '"' -> parsePosition (count + 2) (i + 1)
            | _ -> parsePosition (count + 1) (i + 1)

    parsePosition 2 0

let solvePart1 lines =
   lines
   |> Seq.sumBy (fun str -> String.length str - getStrLength str)

let solvePart2 lines =
   lines
   |> Seq.sumBy (fun str -> encodeString str - String.length str)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }