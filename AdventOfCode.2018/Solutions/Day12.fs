module Year2018Day12

open CameronAavik.AdventOfCode.Common
open System.IO

let parsePlants lines =
    let initState = lines |> Seq.head |> splitBy ": " (fun line -> line.[1])
    let parseRule = splitBy " => " (fun r -> (r.[0], r.[1]))
    let rules =
        lines
        |> Seq.skip 2
        |> Seq.map parseRule
        |> Map.ofSeq
    initState, rules

let step getNextState (curState, startIndex) =
    let paddedState = "...." + curState + "...."
    let nextState =
        paddedState
        |> Seq.windowed 5
        |> Seq.map getNextState
        |> String.concat ""
    let firstPlant, lastPlant = nextState.IndexOf("#"), nextState.LastIndexOf("#")
    let newIndex = startIndex - 2 + firstPlant
    nextState.Substring(firstPlant, lastPlant - firstPlant + 1), newIndex

let getRule rules (rule : char []) = Map.find (new System.String(rule)) rules
let getScore (state, startIndex) =
    state
    |> Seq.mapi (fun i v -> (i, v))
    |> Seq.fold (fun score (i, pot) -> score + (if pot = '#' then i + startIndex else 0)) 0

let solvePart1 (initState, rules) =
    Seq.init 20 id
    |> Seq.fold (fun s _ -> step (getRule rules) s) (initState, 0)
    |> getScore

let solvePart2 (initState, rules) =
    let rec findRepeat i state startIndex =
        let (nextState, nextStartIndex) = step (getRule rules) (state, startIndex)
        if state = nextState then
            let curScore = getScore (nextState, nextStartIndex)
            let scoreDiff = curScore - (getScore (state, startIndex))
            let stepsLeft = (50000000000L - (int64 (i + 1)))
            stepsLeft * (int64 scoreDiff) + (int64 curScore)
        else
            findRepeat (i + 1) nextState nextStartIndex
    findRepeat 0 initState 0

let solver = {parse = File.ReadLines >> parsePlants; part1 = solvePart1; part2 = solvePart2}