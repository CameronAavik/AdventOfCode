module Year2018Day01

open AdventOfCode.FSharp.Common

let (%!) a b = (a % b + b) % b

let solvePart2 changes =
    let shift = Seq.sum changes
    let getDiff ((xi, xf), (yi, yf)) = if shift > 0 then (yf - xf, xi, yf) else (yf - xf, yi, xf)
    Seq.toArray changes
    |> Array.scan (+) 0
    |> Array.take (Seq.length changes)
    |> Array.mapi (fun i v -> (i, v))
    |> Array.groupBy (fun x -> (snd x) %! shift)
    |> Array.map(fun g -> snd g |> Array.sort |> Array.pairwise |> Array.map getDiff)
    |> Array.concat
    |> Array.min

let solver = {parse = parseEachLine asInt; part1 = Seq.sum; part2 = solvePart2 >> (fun (_, _, f) -> f)}