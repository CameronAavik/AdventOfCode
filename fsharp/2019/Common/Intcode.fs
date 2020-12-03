namespace AdventOfCode.FSharp.Y2019.Common

open AdventOfCode.FSharp.Common

module Intcode =
    let parseIntCode = parseFirstLine (splitBy "," asInt64Array)

    type Computer =
        { PC : int
          RelBase : int
          Memory : Map<int, int64> }

        static member pc c = c.PC
        static member jmp i c = { c with PC = i }
        static member jmpBy offset c = { c with PC = c.PC + offset }
        static member relBase c = c.RelBase
        static member adjustRelBase i c = { c with RelBase = c.RelBase + i }
        static member get i c = Map.tryFind i c.Memory |> Option.defaultValue 0L
        static member set i v c =
            if v = 0L then { c with Memory = Map.remove i c.Memory }
            else { c with Memory = Map.add i v c.Memory }
        static member create intcode =
            ({ PC = 0; RelBase = 0; Memory = Map.empty }, Array.indexed intcode)
            ||> Array.fold (fun c (i, v) -> Computer.set i v c)

    [<NoComparison>]
    [<NoEquality>]
    type ProgramState =
        | Output of int64 list * ProgramState
        | Input of (int64 -> ProgramState)
        | Halted

    [<NoComparison>]
    [<NoEquality>]
    [<RequireQualifiedAccess>]
    type InstructionResult =
        | Complete of Computer
        | WithOutput of int64 * Computer
        | NeedsInput of (int64 -> Computer)
        | Halted

    type Mode = Position | Immediate | Relative

    type Instruction<'T> =
        | Add of 'T * 'T * 'T
        | Mul of 'T * 'T * 'T
        | Inp of 'T
        | Out of 'T
        | Jnz of 'T * 'T
        | Jez of 'T * 'T
        | Lt of 'T * 'T * 'T
        | Eq of 'T * 'T * 'T
        | Adj of 'T
        | Hlt

        static member Decode (n : int) =
            let parseMode = function
                | 0 -> Position
                | 1 -> Immediate
                | 2 -> Relative
                | c -> failwithf "Invalid mode: %i" c

            let getMode d = parseMode ((n / d) % 10)
            let a1, a2, a3 = getMode 100, getMode 1000, getMode 10000
            match n % 100 with
            | 1 -> Add (a1, a2, a3)
            | 2 -> Mul (a1, a2, a3)
            | 3 -> Inp a1
            | 4 -> Out a1
            | 5 -> Jnz (a1, a2)
            | 6 -> Jez (a1, a2)
            | 7 -> Lt  (a1, a2, a3)
            | 8 -> Eq  (a1, a2, a3)
            | 9 -> Adj a1
            | 99 -> Hlt
            | _ -> failwithf "Invalid instruction: %i" n

        static member MapIndexed f = function
            | Add (x, y, z) -> Add (f 1 x, f 2 y, f 3 z)
            | Mul (x, y, z) -> Mul (f 1 x, f 2 y, f 3 z)
            | Inp x -> Inp (f 1 x)
            | Out x -> Out (f 1 x)
            | Jnz (x, y) -> Jnz (f 1 x, f 2 y)
            | Jez (x, y) -> Jez (f 1 x, f 2 y)
            | Lt (x, y, z) -> Lt (f 1 x, f 2 y, f 3 z)
            | Eq (x, y, z) -> Eq (f 1 x, f 2 y, f 3 z)
            | Adj x -> Adj (f 1 x)
            | Hlt -> Hlt

        static member Length = function
            | Add _ | Mul _ | Lt _ | Eq _ -> 4
            | Jnz _ | Jez _ -> 3
            | Inp _ | Out _ | Adj _ -> 2
            | Hlt -> 1

    let runInstruction comp =
        let pc = Computer.pc comp
        let getMem i = Computer.get i comp

        let rec modeToPtr i = function
            | Immediate -> pc + i
            | Position -> getMem (pc + i) |> int
            | Relative -> modeToPtr i Position + Computer.relBase comp

        let applyInstruction = 
            function
            | Add (x, y, z) -> Computer.set z (getMem x + getMem y) >> InstructionResult.Complete
            | Mul (x, y, z) -> Computer.set z (getMem x * getMem y) >> InstructionResult.Complete
            | Inp x -> (fun c i -> Computer.set x i c) >> InstructionResult.NeedsInput
            | Out x -> (fun c -> (getMem x), c) >> InstructionResult.WithOutput
            | Jnz (x, y) -> (if getMem x <> 0L then Computer.jmp (getMem y |> int) else id) >> InstructionResult.Complete
            | Jez (x, y) -> (if getMem x  = 0L then Computer.jmp (getMem y |> int) else id) >> InstructionResult.Complete
            | Lt (x, y, z) -> Computer.set z (if getMem x < getMem y then 1L else 0L) >> InstructionResult.Complete
            | Eq (x, y, z) -> Computer.set z (if getMem x = getMem y then 1L else 0L) >> InstructionResult.Complete
            | Adj x -> Computer.adjustRelBase (getMem x |> int) >> InstructionResult.Complete
            | Hlt -> (fun _ -> InstructionResult.Halted)

        let instruction =
            int (getMem pc)
            |> Instruction<_>.Decode
            |> Instruction<_>.MapIndexed modeToPtr

        comp 
        |> Computer.jmpBy (Instruction<_>.Length instruction)
        |> applyInstruction instruction

    let rec run comp =
        match runInstruction comp with
        | InstructionResult.Complete comp -> run comp
        | InstructionResult.WithOutput (x, comp) ->
            match run comp with
            | Output (xs, comp') -> Output (x :: xs, comp')
            | s -> Output ([x], s)
        | InstructionResult.NeedsInput f -> Input (f >> run)
        | InstructionResult.Halted -> Halted
