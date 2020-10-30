module Year2016Day04

open AdventOfCode.FSharp.Common

type Room = {name: string; sector: int; check: string}

let parseRoom (groups : string[]) =
    let name = groups.[0] |> Seq.filter (fun l -> l <> '-') |> Seq.map string |> String.concat ""
    let sector = int groups.[1]
    let check = groups.[2]
    {name=name; sector=sector; check=check}

let parse = parseEachLine (withRegex "(.*)-(\d+)\[(.*)\]") >> Seq.map parseRoom

let rot n str =
    let toInt (c : char) = (int c - int 'a')
    let toChar (i : int) = char (i + int 'a')
    str |> Seq.map (toInt >> (fun i -> (i + n) % 26) >> toChar >> string) |> String.concat ""

let getCheck name =
    let getCheck' =
        Seq.countBy id
        >> Seq.sortBy (fun (k, b) -> (-b, k))
        >> Seq.take 5
        >> Seq.map (fun (k, _) -> string k)
        >> String.concat ""
    getCheck' name

let solvePart1 lines =
    lines
    |> Seq.filter (fun s -> s.check = (getCheck s.name))
    |> Seq.sumBy (fun s -> s.sector)

let solvePart2 lines =
    let line = lines |> Seq.find (fun line -> (rot line.sector line.name).Contains("north"))
    line.sector

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }