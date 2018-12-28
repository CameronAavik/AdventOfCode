module Year2016Day05

open CameronAavik.AdventOfCode.Common
open System.Security.Cryptography

let parse = parseFirstLine asString

let md5 (msg : string) : string =
    use md5 = MD5.Create()
    msg
    |> System.Text.Encoding.ASCII.GetBytes
    |> md5.ComputeHash
    |> Seq.map (fun c -> c.ToString("x2"))
    |> Seq.reduce ( + )

let hash salt number = salt + (string number) |> md5

let isValidHash (h : string) = h.StartsWith("00000")

let solvePart1 doorId =
    Seq.initInfinite (hash doorId)
    |> Seq.filter isValidHash
    |> Seq.take 8
    |> Seq.map (fun h -> h.[5])
    |> charsToStr

let isValidIndex idx = '0' <= idx && idx < '8'

let solvePart2 doorId = 
    let rec searchNext count seen mapping =
        let h = hash doorId count
        let index, c = h.[5], h.[6]
        if isValidHash h && isValidIndex index && not (Set.contains index seen) then
            let newSeen = Set.add index seen
            let newMap = Map.add index c mapping
            if Set.count newSeen = 8 then
                newMap
            else
                searchNext (count + 1) newSeen newMap
        else
            searchNext (count + 1) seen mapping
    let positions = searchNext 0 Set.empty Map.empty |> Map.toArray
    let values = positions |> Array.map snd
    let inds = positions |> Array.map (fun (i, _) -> i |> string |> int)
    Array.permute (fun i -> inds.[i]) values |> charsToStr

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }