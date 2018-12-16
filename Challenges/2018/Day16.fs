module Year2018Day16

open CameronAavik.AdventOfCode.Common

type Registers = int * int * int * int
let get i ((r0, r1, r2, r3) : Registers) =
    match i with
    | 0 -> r0
    | 1 -> r1
    | 2 -> r2
    | 3 -> r3
    | _ -> failwithf "Invalid Register %i" i

let set i v ((r0, r1, r2, r3) : Registers) =
    match i with
    | 0 -> (v, r1, r2, r3)
    | 1 -> (r0, v, r2, r3)
    | 2 -> (r0, r1, v, r3)
    | 3 -> (r0, r1, r2, v)
    | _ -> failwithf "Invalid Register %i" i

let parseRegSample (line : string) =
    let ints = line.Substring(9, 10) |> splitBy ", " asIntArray
    ints.[0], ints.[1], ints.[2], ints.[3]

type Instruction = {opcode: int; args: int * int * int}
let parseInstr line =
    let ints = splitBy " " asIntArray line
    {opcode=ints.[0]; args=(ints.[1], ints.[2], ints.[3])}

type Sample = Registers * Instruction * Registers
let parseSample line1 line2 line3 : Sample =
    let before = parseRegSample line1
    let instr = parseInstr line2
    let after = parseRegSample line3
    before, instr, after

let parseInput lines =
    let lineList = lines |> Seq.toList
    let rec getSamples lines samples =
        match lines with
        | before :: inst :: after :: _ :: next :: ls ->
            let sample = parseSample before inst after
            if next = "" then
                sample :: samples, ls
            else
                getSamples (next :: ls) (sample :: samples)
        | _ -> failwithf "Unexpected end of file"
    let samples, linesAfter = getSamples lineList []
    let instrs = List.map parseInstr (List.tail linesAfter)
    samples, instrs

// instruction helpers
let inst f v1 v2 c = set c (f v1 v2)
let instir f (a, b, c) regs = inst f a (get b regs) c regs
let instri f (a, b, c) regs = inst f (get a regs) b c regs
let instrr f (a, b, c) regs = inst f (get a regs) (get b regs) c regs

let useFirst a _ = a
let gt a b = if a > b then 1 else 0
let eq a b = if a = b then 1 else 0

// instructions
let addr = instrr (+)
let addi = instri (+)
let mulr = instrr (*)
let muli = instri (*)
let banr = instrr (&&&)
let bani = instri (&&&)
let borr = instrr (|||)
let bori = instri (|||)
let setr = instri useFirst
let seti = instir useFirst
let gtir = instir gt
let gtri = instri gt
let gtrr = instrr gt
let eqir = instir eq
let eqri = instri eq
let eqrr = instrr eq

type InstructionHandler = int * int * int -> Registers -> Registers
let instructions : InstructionHandler [] =
    [| addr; addi; mulr; muli; banr; bani; borr; bori;
      setr; seti; gtir; gtri; gtrr; eqir; eqri; eqrr |]

let couldBeInstruction (before, {args=args}, after) inst = inst args before = after
let couldBeThreeOrMoreInstructions sample =
    let potentials = instructions |> Array.filter (couldBeInstruction sample)
    Array.length potentials >= 3

let solvePart1 (samples : Sample list, _) =
    samples
    |> List.filter couldBeThreeOrMoreInstructions
    |> List.length

let allCanBeInstruction samples inst =
    samples 
    |> List.exists (fun sample -> not <| couldBeInstruction sample inst) 
    |> not

let possibleInstructions samples =
    instructions
    |> Array.mapi (fun i v -> (i, v))
    |> Array.filter (snd >> allCanBeInstruction samples)
    |> Array.map fst
    |> Set.ofArray

let getAllPossibleOpMappings =
    List.groupBy (fun (_, i, _) -> i.opcode)
    >> List.sortBy fst
    >> List.map (snd >> possibleInstructions)

// this is a messy recursive brute-force approach to finding a valid combination
let getOpMapping samples =
    let possibleOpMappings = getAllPossibleOpMappings samples
    let rec findMapping mapping seen =
        function
        | funcs :: ms ->
            let choices = Set.difference funcs seen |> Set.toList
            let rec findChoice =
                function
                | [] -> false, []
                | c :: cs ->
                    let isFound, mapping = findMapping (c :: mapping) (Set.add c seen) ms
                    if isFound then
                        true, mapping
                    else
                        findChoice cs
            findChoice choices
        | [] -> true, mapping
    findMapping [] Set.empty possibleOpMappings
    |> snd
    |> List.rev
    |> List.toArray
    |> Array.map (fun i -> instructions.[i])

let solvePart2 (samples : Sample list, instructions) =
    let ops = getOpMapping samples
    let applyInstruction registers instruction =
        let inst = ops.[instruction.opcode]
        inst instruction.args registers
    List.fold applyInstruction (0, 0, 0, 0) instructions
    |> get 0

let solver = {parse = parseInput; part1 = solvePart1; part2 = solvePart2}