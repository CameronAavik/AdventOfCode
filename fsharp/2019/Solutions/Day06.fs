module Year2019Day06

open AdventOfCode.FSharp.Common

let asOrbit line =
    let objects = splitBy ")" id line
    objects.[1], objects.[0]

let parse = parseEachLine asOrbit >> Map.ofSeq

let pathToRoot orbits object =
    let rec pathToRoot' object =
        match Map.tryFind object orbits with
        | Some obj -> obj :: pathToRoot' obj
        | None -> [ ]
    pathToRoot' object
    
let solvePart1 orbits =
    orbits
    |> Map.toSeq
    |> Seq.sumBy (fst >> pathToRoot orbits >> List.length)
    
let solvePart2 orbits = 
    let pathToYou = pathToRoot orbits "YOU" |> List.rev
    let pathToSanta = pathToRoot orbits "SAN" |> List.rev

    let distToCommonParent = Seq.zip pathToYou pathToSanta |> Seq.findIndex (fun (obj1, obj2) -> obj1 <> obj2)
    pathToYou.Length + pathToSanta.Length - (2 * distToCommonParent)

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }