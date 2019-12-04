module Year2019Day02

open CameronAavik.AdventOfCode.Common

type ProgramState = { PC : int; IntCode : Map<int, int>; Halt : bool }
let get i state = state.IntCode.[i]
let set i x state = { state with IntCode = Map.add i x state.IntCode }
let incPC state = { state with PC = state.PC + 4 }

let step state =
    match get state.PC state with
    | 1 | 2 as op -> 
        let x = get (state.PC + 1) state
        let y = get (state.PC + 2) state
        let t = get (state.PC + 3) state
        let f = if op = 1 then (+) else (*)
        set t (f (get x state) (get y state)) state
        |> incPC
    | 99 -> { state with Halt = true }
    | c -> failwithf "Invalid opcode: %i" c

let rec stepUntilHalt s =
    if s.Halt then s
    else step s |> stepUntilHalt

// converts array to Map where key is the array index
let intCodeToMap intCode = intCode |> Array.indexed |> Map.ofArray

let solve noun verb intCodeMap =
    let intCodeMap = intCodeMap |> Map.add 1 noun |> Map.add 2 verb
    stepUntilHalt { PC = 0; IntCode = intCodeMap; Halt = false } |> get 0

let solvePart1 intCode = solve 12 2 (intCodeToMap intCode)
    
let solvePart2 intCode = 
    let intCodeMap = intCodeToMap intCode
    seq {
        for noun = 0 to 99 do
            for verb = 0 to 99 do
                if solve noun verb intCodeMap = 19690720 then
                    100 * noun + verb } |> Seq.head

let solver = { parse = parseFirstLine extractInts; part1 = solvePart1; part2 = solvePart2 }