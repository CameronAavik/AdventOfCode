module Year2015Day04

open CameronAavik.AdventOfCode.Common
open System.Security.Cryptography
open System.Text

let parse = parseFirstLine asString

let getHash (input : string) =
    let md5Obj = MD5.Create()
    let inputAsBytes = Encoding.ASCII.GetBytes input
    let getHash' index =
        let indexBytes = Encoding.ASCII.GetBytes (string index)
        let byteArr = Array.append inputAsBytes indexBytes
        md5Obj.ComputeHash byteArr
    getHash'

let solvePart1 input =
    Seq.initInfinite (getHash input)
    |> Seq.indexed
    |> Seq.find (fun (i, h) -> h.[0] = 0uy && h.[1] = 0uy && h.[2] &&& 0xF0uy = 0uy)
    |> fst

let solvePart2 input =
    Seq.initInfinite (getHash input)
    |> Seq.indexed
    |> Seq.find (fun (i, h) -> h.[0] = 0uy && h.[1] = 0uy && h.[2] = 0uy)
    |> fst

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }