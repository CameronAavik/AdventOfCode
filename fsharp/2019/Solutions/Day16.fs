module Year2019Day16

open AdventOfCode.FSharp.Common

let parse = parseFirstLine (Seq.map (fun c -> (int c - int '0')) >> Seq.toArray)

let applyFFTPhase basePos list =
    let len = Array.length list
    let preSum = Array.scan (+) 0 list
    let finalSum = Array.last preSum

    let getValAtIndex i =
        let pos = i + basePos
        let rec getVal' j pat acc =
            if j >= len then acc
            elif j + pos >= len then acc + pat * (finalSum - preSum.[j])
            else getVal' (j + 2 * pos) (-pat) (acc + pat * (preSum.[j + pos] - preSum.[j]))
        getVal' i 1 0

    let toLastDigit i = abs (i % 10)

    Array.init len (getValAtIndex >> toLastDigit)

let rec solve basePos n list =
    if n = 0 then list
    else applyFFTPhase basePos list |> solve basePos (n - 1)

let getFirstNDigits n = Array.take n >> Array.reduce (fun i n -> 10 * i + n)

let solvePart1 = solve 1 100 >> getFirstNDigits 8

let solvePart2 input =
    let offset = getFirstNDigits 7 input
    let len = Array.length input
    Array.init (len * 10000 - offset) (fun i -> input.[(offset + i) % len])
    |> solve (offset + 1) 100
    |> getFirstNDigits 8

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }