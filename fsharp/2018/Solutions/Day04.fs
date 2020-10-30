module Year2018Day04

open AdventOfCode.FSharp.Common
open System.IO

let getOrDefault key map ``default``= 
    match Map.tryFind key map with
    | Some v -> v
    | None -> ``default``

type Activity = StartShift of int | Wake | Sleep
let asLog (str : string) =
    let parts = str.Split(":] ".ToCharArray())
    let activity =
        match parts.[4] with
        | "Guard" -> StartShift (int parts.[5].[1..])
        | "wakes" -> Wake
        | "falls" -> Sleep
        | _ -> failwithf "Invalid Activity %s" parts.[4]
    int parts.[2], activity
        
type SleepState = Awake | Asleep of int
type State = {guard: int; state: SleepState; sleeps: Map<int, (int * int) list>}
let step {guard=guard; state=state; sleeps=sleeps} =
    function
    | (_, StartShift _id) -> {guard=_id; state=Awake; sleeps=sleeps}
    | (minute, Sleep) -> {guard=guard; state=Asleep minute; sleeps=sleeps}
    | (minute, Wake) ->
        let newSleeps =
            match state with
            | Awake -> sleeps
            | Asleep time ->
                let currentSleeps = getOrDefault guard sleeps []
                Map.add guard ((time, minute)::currentSleeps) sleeps
        {guard=guard; state=Awake; sleeps=newSleeps}

let logToSleeps = Seq.fold step {guard=0; state=Awake; sleeps=Map.empty} >> (fun s -> s.sleeps)
let findSleepiest = Map.toSeq >> Seq.maxBy (snd >> List.sumBy (fun (start, stop) -> stop - start)) >> fst
let stepSleeps (maxMin, maxCount, curCount) (minute, isSleeping) =
    if isSleeping then
        if curCount = maxCount then (minute, curCount + 1, curCount + 1)
        else (maxMin, maxCount, curCount + 1)
    else (maxMin, maxCount, curCount - 1)
let findMaxSleep =
    Seq.map (fun (a, b) -> [(a, true); (b, false)])
    >> Seq.concat
    >> Seq.sort
    >> Seq.fold stepSleeps (0, 0, 0)
    >> (fun (minute, duration, _) -> minute, duration)
        
let solvePart1 logs =
    let sleeps = logToSleeps logs
    let sleepiest = findSleepiest sleeps
    let maxSleep, _ = Map.find sleepiest sleeps |> findMaxSleep
    sleepiest * maxSleep

let solvePart2 =
    logToSleeps
    >> Map.toSeq
    >> Seq.map (fun (k, v) -> (k, findMaxSleep v))
    >> Seq.maxBy (snd >> snd)
    >> (fun (guard, (minute, _)) -> guard * minute)

let solver = {parse = File.ReadLines >> Seq.sort >> Seq.map asLog; part1 = solvePart1; part2 = solvePart2}