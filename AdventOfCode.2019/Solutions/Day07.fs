module Year2019Day07

open CameronAavik.AdventOfCode.Common
open CameronAavik.AdventOfCode.Y2019.Common
open CameronAavik.AdventOfCode.Y2019.Common.IntCodeVM

let rec permutations items =
    seq { 
        if Set.isEmpty items then []
        else
            for x in items do
                for xs in permutations (Set.remove x items) do
                    x :: xs }

let runAmpWithSignal signal =
    mapIO (IOQueues.writeToInput signal)
    >> run
    >> readOutputFromQueue

type AmpState = Running of signal: int64 | Completed of signal: int64

let runAmpInState state amp =
    match state with
    | Running i -> 
        let outputSignal, newAmpState = runAmpWithSignal i amp
        match outputSignal with 
        | Some o -> newAmpState, Running o 
        | None   -> newAmpState, Completed i
    | Completed _ -> amp, state

let runAllAmps state amps =
    Array.mapFold runAmpInState state amps

let processAmps1 state amps =
    match runAllAmps state amps with
    | _, Running i -> i
    | _, Completed i -> i

let rec processAmps2 state amps =
    match runAllAmps state amps with
    | _, Completed s -> s
    | newAmps, state' -> processAmps2 state' newAmps

let solve processAmps minId maxId intCode =
    let program = bootProgram (QueueIO.create) intCode
    let getThrusterSignal (ampIds : int64 []) =
        Array.init 5 (fun i -> program |> writeInputToQueue (ampIds.[i]))
        |> processAmps (Running 0L)

    permutations (set [minId .. maxId])
    |> Seq.map (List.toArray >> getThrusterSignal)
    |> Seq.max

let solver = { parse = parseIntCodeFromFile; part1 = solve processAmps1 0L 4L; part2 = solve processAmps2 5L 9L }