namespace CameronAavik.AdventOfCode.Y2019.Common

open CameronAavik.AdventOfCode.Common
open FSharpx.Collections

module IntCodeVM =
    let parseIntCodeFromFile = 
        parseFirstLine (splitBy "," asIntArray)
        >> Array.indexed
        >> Map.ofArray

    type ProgramState = { PC : int; IntCode : Map<int, int>; Input : Queue<int>; Output : Queue<int>; IsHalted : bool }
    let bootProgram intcode = { PC = 0; IntCode = intcode; Input = Queue.empty; Output = Queue.empty; IsHalted = false }
    
    type Parameter = Position of int | Immediate of int
    let parsePosition mode = if mode = 0 then Position else Immediate
    
    let getValue param state = 
        match param with
        | Position i -> state.IntCode.[i]
        | Immediate i -> i
    
    let setValue param x state =
        match param with
        | Position i -> { state with IntCode = Map.add i x state.IntCode }
        | Immediate i -> failwithf "Tried to set value at address %i via immediate mode" i

    let writeToInput value state = { state with Input = Queue.conj value state.Input }
    let readFromInput addr state =
        match state.Input with
        | Queue.Cons (i, is) -> setValue addr i { state with Input = is }
        | Queue.Nil -> failwith "Tried to read input from empty buffer"
    
    let writeToOutput addr state = { state with Output = Queue.conj (getValue addr state) state.Output }
    let readFromOutput state =
        match Queue.tryUncons state.Output with
        | Some i -> i
        | None -> failwith "Tried to read output from empty buffer"
    let readAllOutput state = Queue.toSeq state.Output
    
    let incPC i state = { state with PC = state.PC + i }
    let jmpPC i state = { state with PC = (getValue i state) }
    
    type Operation =
        | Add of Parameter * Parameter * Parameter
        | Mult of Parameter * Parameter * Parameter
        | Input of Parameter
        | Output of Parameter
        | JmpNotZero of Parameter * Parameter
        | JmpEqZero of Parameter * Parameter
        | SetIfLessThan of Parameter * Parameter * Parameter
        | SetIfEq of Parameter * Parameter * Parameter
        | Halt
    
    let getOperation state =
        let instruction = getValue (Position state.PC) state
        let opCode = instruction % 100
        let paramModes = [| 100; 1000; 10000 |] |> Array.map (fun d -> parsePosition (instruction / d % 10))
    
        let param i = getValue (Position (state.PC + i + 1)) state |> paramModes.[i]
        let with1Param () = param 0
        let with2Params () = (param 0, param 1)
        let with3Params () = (param 0, param 1, param 2)
    
        match opCode % 100 with
        | 1 -> Add (with3Params ())
        | 2 -> Mult (with3Params ())
        | 3 -> Input (with1Param ())
        | 4 -> Output (with1Param ())
        | 5 -> JmpNotZero (with2Params ())
        | 6 -> JmpEqZero (with2Params ())
        | 7 -> SetIfLessThan (with3Params ())
        | 8 -> SetIfEq (with3Params ())
        | 99 -> Halt
        | _ -> failwithf "Invalid Opcode: %i" instruction
    
    let step state =
        match getOperation state with
        | Add (x, y, t) -> setValue t ((getValue x state) + (getValue y state)) state |> incPC 4
        | Mult (x, y, t) -> setValue t ((getValue x state) * (getValue y state)) state |> incPC 4
        | Input addr -> readFromInput addr state |> incPC 2
        | Output addr -> writeToOutput addr state |> incPC 2
        | JmpNotZero (t, addr) -> if getValue t state <> 0 then jmpPC addr state else incPC 3 state
        | JmpEqZero (t, addr) -> if getValue t state = 0 then jmpPC addr state else incPC 3 state
        | SetIfLessThan (x, y, t) -> setValue t (if (getValue x state) < (getValue y state) then 1 else 0) state |> incPC 4
        | SetIfEq (x, y, t) -> setValue t (if (getValue x state) = (getValue y state) then 1 else 0) state |> incPC 4
        | Halt -> { state with IsHalted = true }
    
    let rec runUntilHalt s =
        if s.IsHalted then s
        else step s |> runUntilHalt