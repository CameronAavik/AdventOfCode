module Year2016Day05

open CameronAavik.AdventOfCode.Common
open System.Security.Cryptography

let parse = parseFirstLine asString
let md5Obj = MD5.Create()

let asHexDigit num = 
    match num with
    | num when num < 10uy -> '0' + char num
    | _ -> 'a' + char (num - 10uy)

let md5StartsWith5Zeroes (msg : string) : bool * char * char =
    let hash =
        msg
        |> System.Text.Encoding.ASCII.GetBytes
        |> md5Obj.ComputeHash
    let isValid = hash.[0] = 0uy && hash.[1] = 0uy && (hash.[2] &&& 0xF0uy) = 0uy
    isValid, asHexDigit (hash.[2] &&& 0x0Fuy), asHexDigit (hash.[3] >>> 4)

let hashStartsWith5Zeroes salt number = salt + (string number) |> md5StartsWith5Zeroes

let solvePart1 doorId =
    Seq.initInfinite (hashStartsWith5Zeroes doorId)
    |> Seq.filter (fun (isValid, _, _) -> isValid)
    |> Seq.take 8
    |> Seq.map (fun (_, d, _) -> d)
    |> charsToStr

let isValidIndex idx = '0' <= idx && idx < '8'

let solvePart2 doorId = 
    let rec searchNext count seen mapping =
        let isValidHash, index, c = hashStartsWith5Zeroes doorId count
        if isValidHash && isValidIndex index && not (Set.contains index seen) then
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