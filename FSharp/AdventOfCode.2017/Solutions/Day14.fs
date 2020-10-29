module Year2017Day14

open CameronAavik.AdventOfCode.Common
open System

let toBinStr (i : int) =
    Convert.ToString(i, 2).PadLeft(8, '0')

let getHash key i = 
    Year2017Day10.strToDenseHash (sprintf "%s-%i" key i) 
    |> Array.fold (fun h i -> h + toBinStr i) ""

let hashToCoords i = 
    Seq.mapi (fun j h -> ((i, j), h)) 
    >> Seq.filter (snd >> ((=) '1')) 
    >> Seq.map fst
    >> Set.ofSeq

let getActiveCoords key = 
    Seq.map (getHash key) [0..127]
    |> Seq.mapi hashToCoords
    |> Set.unionMany

let getSurroundingNodes activeCoords (i, j) =
    [(i-1, j); (i+1, j); (i, j-1); (i, j+1)]
    |> List.filter (fun node -> Set.contains node activeCoords)

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

let part2 =
    getActiveCoords
    >> (fun coords -> getConnectedComponents (getSurroundingNodes coords) coords)

let solver = {parse = parseFirstLine asString; part1 = getActiveCoords >> Set.count; part2 = part2 >> List.length}