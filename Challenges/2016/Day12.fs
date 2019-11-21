module Year2016Day12

open System
open CameronAavik.AdventOfCode.Common

type Register = string

type Value =
    | RegVal of Register
    | IntVal of int

type Operation = 
    | Copy of Value * Register
    | Incr of Register
    | Decr of Register
    | JmpNotZero of Value * Value
    | Add of Register * Register * Register
    | Noop

let isReg part = part = "a" || part = "b" || part = "c" || part = "d"

let parseReg reg =
    match reg with
    | "a" | "b" | "c" | "d" -> reg
    | _ -> failwithf "Invalid register: %s" reg
    
let parseVal value = 
    match value with 
    | "a" | "b" | "c" | "d" -> RegVal (parseReg value)
    | _ -> IntVal (Int32.Parse value)

let asInstr (line : string) =
    let parts = line.Split(' ')
    match parts.[0] with
    | "cpy" -> Copy (parseVal parts.[1], parseReg parts.[2])
    | "inc" -> Incr (parseReg parts.[1])
    | "dec" -> Decr (parseReg parts.[1])
    | "jnz" -> JmpNotZero (parseVal parts.[1], parseVal parts.[2])
    | _ -> failwithf "Invalid instruction: %s" line

let parse = parseEachLine asInstr >> Seq.toArray

type Program = 
    { Code : Operation [] 
      PC : int
      Registers : Map<Register, int> }

let newProgram code =
    { Code=code; PC=0; Registers = Map.ofList [("a", 0); ("b", 0); ("c", 0); ("d", 0)]}

let step program =
    let instr = program.Code.[program.PC]
    let getVal value = 
        match value with
        | RegVal r -> program.Registers.[r]
        | IntVal v -> v
    match instr with
    | Copy (v, rTo) -> { program with PC = program.PC + 1; Registers = Map.add rTo (getVal v) program.Registers }
    | Incr r -> { program with PC = program.PC + 1; Registers = Map.add r (getVal (RegVal r) + 1) program.Registers }
    | Decr r -> { program with PC = program.PC + 1; Registers = Map.add r (getVal (RegVal r) - 1) program.Registers }
    | JmpNotZero (t, d) ->
        let jmpDst = if getVal t = 0 then 1 else (getVal d)
        { program with PC = program.PC + jmpDst }
    | Add (r1, r2, r3) -> { program with PC = program.PC + 1; Registers = Map.add r3 (getVal (RegVal r1) + getVal (RegVal r2)) program.Registers }
    | Noop -> { program with PC = program.PC + 1 }
            
let solvePart1 instrs =
    let rec run program =
        if program.PC < 0 || program.PC >= program.Code.Length then program
        else run (step program)
    let program = newProgram instrs
    let completed = run program
    completed.Registers.["a"]

let optimisePass (instrs : Operation []) =
    for idx = 0 to (instrs.Length - 3) do
        match instrs.[idx], instrs.[idx + 1], instrs.[idx + 2] with
        | (Incr r1, Decr r2, JmpNotZero (RegVal r3, IntVal -2)) when r2 = r3 ->
            instrs.[idx] <- Add (r1, r2, r1)
            instrs.[idx + 1] <- Copy (IntVal 0, r2)
            instrs.[idx + 2] <- Noop
        | _ -> ()

let solvePart2 (instrs : Operation []) =
    let newInstrs = ((Copy (IntVal 1, "c")) :: (List.ofArray instrs)) |> Array.ofList
    optimisePass newInstrs
    solvePart1 newInstrs

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }