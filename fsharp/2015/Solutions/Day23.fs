module Year2015Day23

open AdventOfCode.FSharp.Common

type Instruction =
    | Half of string
    | Triple of string
    | Increment of string
    | Jump of int
    | JumpIfEven of string * int
    | JumpIfOne of string * int

let asInstr line =
    let toks = splitBy " " id line
    match toks.[0] with
    | "hlf" -> Half toks.[1]
    | "tpl" -> Triple toks.[1]
    | "inc" -> Increment toks.[1]
    | "jmp" -> Jump (int toks.[1])
    | "jie" -> JumpIfEven (toks.[1].TrimEnd(','), int toks.[2])
    | "jio" -> JumpIfOne (toks.[1].TrimEnd(','), int toks.[2])
    | _ -> failwithf "Invalid Instruction: %s" line

let parse = parseEachLine asInstr >> Seq.toArray

type State = { PC : int; Regs : Map<string, int>; Code : Instruction [] }
let setReg reg value state = { state with Regs = Map.add reg value state.Regs }
let getReg reg state = Map.tryFind reg state.Regs |> Option.defaultValue 0
let mapReg f reg state = state |> setReg reg (getReg reg state |> f)
let jmp offset state = { state with PC = state.PC + offset }

let step state =
    match state.Code.[state.PC] with
    | Half r -> state |> mapReg (fun r -> r / 2) r |> jmp 1
    | Triple r -> state |> mapReg (fun r -> r * 3) r |> jmp 1
    | Increment r -> state |> mapReg ((+) 1) r |> jmp 1
    | Jump o -> state |> jmp o
    | JumpIfEven (r, o) -> state |> jmp (if getReg r state % 2 = 0 then o else 1)
    | JumpIfOne (r, o) -> state |> jmp (if getReg r state = 1 then o else 1)

let runProgram state =
    let rec run' state =
        if state.PC < 0 || state.PC >= state.Code.Length then state
        else step state |> run'
    run' state

let solvePart1 input =
    { PC = 0; Regs = Map.empty; Code = input }
    |> runProgram
    |> getReg "b"

let solvePart2 input =
    { PC = 0; Regs = Map.empty; Code = input }
    |> setReg "a" 1
    |> runProgram
    |> getReg "b"

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }