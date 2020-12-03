module Year2019Day07

open AdventOfCode.FSharp.Common
open AdventOfCode.FSharp.Y2019.Common.Intcode

let rec permutations items =
    seq { 
        if Set.isEmpty items then []
        else
            for x in items do
                for xs in permutations (Set.remove x items) do
                    x :: xs }

let provideInput systemId =
    function
    | Input f -> f systemId
    | s -> s

let tryReadOutput =
    function
    | Output (o, c) -> Some o, c
    | c -> None, c

let runAmpWithSignal signal = provideInput signal >> tryReadOutput

type AmpState = Running of signal: int64 | Completed of signal: int64

let runAmpInState state amp =
    match state with
    | Running i -> 
        match runAmpWithSignal i amp with
        | Some x, c -> c, Running x.[0]
        | None, c -> c, Completed i
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
    let program = Computer.create intCode |> run
    let getThrusterSignal (ampIds : int64 []) =
        Array.init 5 (fun i -> program |> provideInput (ampIds.[i]))
        |> processAmps (Running 0L)

    permutations (set [minId .. maxId])
    |> Seq.map (List.toArray >> getThrusterSignal)
    |> Seq.max

let solver = { parse = parseIntCode; part1 = solve processAmps1 0L 4L; part2 = solve processAmps2 5L 9L }