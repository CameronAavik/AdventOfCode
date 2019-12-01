module Year2017Day12

open CameronAavik.AdventOfCode.Common

let getConnectedComponent getVerts rootNode =
    let rec getConnectedComponent' comp = function
        | [] -> comp
        | x :: xs when Set.contains x comp -> getConnectedComponent' comp xs
        | x :: xs -> getConnectedComponent' (Set.add x comp) (getVerts x @ xs)
    getConnectedComponent' Set.empty [rootNode]

let getConnectedComponents getVerts nodes =
    let rec getConnectedComponents' seen unseen components =
        if Set.isEmpty unseen then components
        else
            let newComp = getConnectedComponent getVerts (Seq.head unseen)
            getConnectedComponents' (Set.union seen newComp) (Set.difference unseen newComp) (newComp :: components)
    getConnectedComponents' Set.empty nodes List.empty

let asConnections = splitBy ", " asIntArray >> Array.toList
let asPipe = splitBy " <-> " (Array.item 1 >> asConnections)
let part1 graph = getConnectedComponent (fun v -> List.item v graph) 0 |> Set.count
let part2 graph = getConnectedComponents (fun v -> List.item v graph) (set [0..(List.length graph - 1)]) |> List.length
let solver = {parse = parseEachLine asPipe >> Seq.toList; part1 = part1; part2 = part2}