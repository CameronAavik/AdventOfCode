module Year2017Day07

open CameronAavik.AdventOfCode.Common

let getSubTowers (tokens : string array) = if Array.length tokens = 2 then Array.empty else tokens.[3..] |> Array.map (fun c -> c.TrimEnd(','))
let asProgram = splitBy " " (fun tokens -> (tokens.[0], (tokens.[1].Trim('(',')') |> int, getSubTowers tokens)))

let rec findRoot tower currentProgram = 
    match Map.tryFindKey (fun _ (_, children) -> Array.contains currentProgram children) tower with
    | None -> currentProgram
    | Some parent -> findRoot tower parent

let rec getWeight tower node = 
    let weight, children = Map.find node tower
    weight + (Array.sumBy (getWeight tower) children)

let getChildrenWeights tower = 
    Seq.map (fun c -> (getWeight tower c, Map.find c tower |> fst)) 
    >> Seq.groupBy fst 
    >> Seq.sortByDescending (fun (k, g) -> Seq.length g) 
    >> Seq.toArray

let getMissingWeight tower = 
    tower 
    |> Map.map (fun _ (_, children) -> getChildrenWeights tower children)
    |> Map.toSeq
    |> Seq.filter (fun (_, v) -> (Array.length v) = 2)
    |> Seq.map (fun (_, v) -> (snd v.[1] |> Seq.head |> snd) + (fst v.[0]) - (fst v.[1]))
    |> Seq.min

let part1 tower = findRoot tower (Seq.head tower).Key
let solver = {parse = parseEachLine asProgram >> Map.ofSeq; part1 = part1; part2 = getMissingWeight}