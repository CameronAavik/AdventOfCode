module Year2015Day14

open CameronAavik.AdventOfCode.Common

type Reindeer = { Name : string; Speed : int; Seconds : int; Rest : int }

let parseReindeer r =
    let toks = splitBy " " id r
    { Name = toks.[0]; Speed = int toks.[3]; Seconds = int toks.[6]; Rest = int toks.[13] }

let parse = parseEachLine parseReindeer

let solvePart1 input =
    let reindeer = input |> Seq.map (fun r -> r.Name, r ) |> Map.ofSeq
    let initStates = reindeer |> Map.map (fun r d -> (0, true, d.Seconds))

    let step states =
        states
        |> Map.map (fun r (dist, isGoing, t) ->
            let reinData = reindeer.[r]
            let newDist =
                if isGoing then dist + reinData.Speed
                else dist
            let newIsGoing = if t = 1 then not isGoing else isGoing
            let newT = if t = 1 then (if newIsGoing then reinData.Seconds else reinData.Rest) else t - 1
            (newDist, newIsGoing, newT))
        
    let rec simulateN n state =
        if n = 0 then state
        else step state |> simulateN (n - 1)

    simulateN 2503 initStates
    |> Map.toSeq
    |> Seq.map (fun (_, (d, _, _)) -> d)
    |> Seq.max

let solvePart2 input =
    let reindeer = input |> Seq.map (fun r -> r.Name, r ) |> Map.ofSeq
    let initStates = reindeer |> Map.map (fun r d -> (0, true, d.Seconds, 0))

    let step states =
        let newStates =
            states
            |> Map.map (fun r (dist, isGoing, t, score) ->
                let reinData = reindeer.[r]
                let newDist =
                    if isGoing then dist + reinData.Speed
                    else dist
                let newIsGoing = if t = 1 then not isGoing else isGoing
                let newT = if t = 1 then (if newIsGoing then reinData.Seconds else reinData.Rest) else t - 1
                (newDist, newIsGoing, newT, score))

        let bestDist = 
            newStates
            |> Map.toSeq
            |> Seq.map (fun (_, (d, _, _, _)) -> d)
            |> Seq.max

        newStates
        |> Map.map (fun r (a, b, c, d) -> (a, b, c, if a = bestDist then d + 1 else d))
        
    let rec simulateN n state =
        if n = 0 then state
        else step state |> simulateN (n - 1)

    simulateN 2503 initStates
    |> Map.toSeq
    |> Seq.map (fun (_, (d, _, _, s)) -> s)
    |> Seq.max

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }