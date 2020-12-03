module Year2016Day23

open AdventOfCode.FSharp.Common
open System

type Register = string

type Value =
    | RegVal of Register
    | IntVal of int

type Operation = 
    | Copy of Value * Value
    | Incr of Register
    | Decr of Register
    | JmpNotZero of Value * Value
    | Add of Register * Register * Register
    | Multiply of Register * Register * Register
    | Toggle of Register
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
    | "cpy" -> Copy (parseVal parts.[1], parseVal parts.[2])
    | "inc" -> Incr (parseReg parts.[1])
    | "dec" -> Decr (parseReg parts.[1])
    | "jnz" -> JmpNotZero (parseVal parts.[1], parseVal parts.[2])
    | "tgl" -> Toggle (parseReg parts.[1])
    | _ -> failwithf "Invalid instruction: %s" line

let parse = parseEachLine asInstr >> Seq.toArray

type Program = 
    { Code : Operation [] 
      PC : int
      Registers : Map<Register, int> }

let newProgram code =
    { Code=code; PC=0; Registers = Map.ofList [("a", 0); ("b", 0); ("c", 0); ("d", 0)] }

let toggleInstruction instr =
    match instr with
    | Copy (v, rTo) -> JmpNotZero (v, rTo)
    | Incr r -> Decr r
    | Decr r -> Incr r
    | JmpNotZero (t, d) -> Copy (t, d)
    | Add (r1, r2, r3) -> Add (r1, r2, r3)
    | Multiply (r1, r2, r3) -> Multiply (r1, r2, r3)
    | Noop -> Noop
    | Toggle x -> Incr x

let step program =
    let instr = program.Code.[program.PC]
    let getVal value = 
        match value with
        | RegVal r -> program.Registers.[r]
        | IntVal v -> v
    match instr with
    | Copy (v, RegVal rTo) -> { program with PC = program.PC + 1; Registers = Map.add rTo (getVal v) program.Registers }
    | Copy (v, rTo) -> { program with PC = program.PC + 1 }
    | Incr r -> { program with PC = program.PC + 1; Registers = Map.add r (getVal (RegVal r) + 1) program.Registers }
    | Decr r -> { program with PC = program.PC + 1; Registers = Map.add r (getVal (RegVal r) - 1) program.Registers }
    | JmpNotZero (t, d) ->
        let jmpDst = if getVal t = 0 then 1 else (getVal d)
        { program with PC = program.PC + jmpDst }
    | Add (r1, r2, r3) -> { program with PC = program.PC + 1; Registers = Map.add r3 (getVal (RegVal r1) + getVal (RegVal r2)) program.Registers }
    | Multiply (r1, r2, r3) -> { program with PC = program.PC + 1; Registers = Map.add r3 (getVal (RegVal r1) * getVal (RegVal r2)) program.Registers }
    | Noop -> { program with PC = program.PC + 1 }
    | Toggle x ->
        let instrToToggle = program.PC + (getVal (RegVal x))
        if instrToToggle >= 0 && instrToToggle < program.Code.Length then
            program.Code.[instrToToggle] <- toggleInstruction program.Code.[instrToToggle]
        { program with PC = program.PC + 1 }
            
let solvePart1 instrs =
    let rec run program =
        if program.PC < 0 || program.PC >= program.Code.Length then program
        else run (step program)
    let newInstrs = Array.append [| Copy (IntVal 7, RegVal "a")|] instrs
    let program = newProgram newInstrs
    let completed = run program
    completed.Registers.["a"]

let optimisePass (instrs : Operation []) =
    for idx = 0 to (instrs.Length - 6) do
        match instrs.[idx], instrs.[idx + 1], instrs.[idx + 2], instrs.[idx + 3], instrs.[idx + 4], instrs.[idx + 5] with
        | (Copy (RegVal r6, RegVal r7), Incr r1, Decr r2, JmpNotZero (_, IntVal -2), Decr r4, JmpNotZero (RegVal r5, IntVal -5)) ->
            instrs.[idx] <- Multiply (r6, r5, r1)
            instrs.[idx + 1] <- Copy (IntVal 0, RegVal r2)
            instrs.[idx + 2] <- Copy (IntVal 0, RegVal r4)
            instrs.[idx + 3] <- Noop
            instrs.[idx + 4] <- Noop
            instrs.[idx + 5] <- Noop
        | _ -> ()

let solvePart2 instrs = 
    let rec run program =
        if program.PC < 0 || program.PC >= program.Code.Length then program
        else run (step program)
    let newInstrs = Array.append [| Copy (IntVal 12, RegVal "a")|] instrs
    optimisePass newInstrs
    let program = newProgram newInstrs
    let completed = run program
    completed.Registers.["a"]

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }