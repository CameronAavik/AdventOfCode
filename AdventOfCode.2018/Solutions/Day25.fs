module Year2018Day25

open CameronAavik.AdventOfCode.Common

let manhattan p0 p1 = Array.zip p0 p1 |> Array.sumBy (fun (c0, c1) -> abs (c0 - c1))
let solve points =
    let rec countComponents count unseen =
        let rec findComponent queue unseen =
            match queue with
            | p0 :: ps ->
                let toAdd, toKeep = List.partition (fun p1 -> manhattan p0 p1 <= 3) unseen
                let newQueue = List.foldBack (fun t q -> t :: q) toAdd ps
                findComponent newQueue toKeep
            | [] -> unseen
        match unseen with
        | p :: _ -> countComponents (count + 1) (findComponent [p] unseen)
        | [] -> count
    countComponents 0 (Seq.toList points)

let solver = {parse = id; part1 = parseEachLine (splitBy "," asIntArray) >> solve; part2 = (fun _ -> "Advent of Code Finished!")}