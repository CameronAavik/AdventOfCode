module Year2015Day07

open AdventOfCode.FSharp.Common

// TODO: Replace with Union-Find

type Value = 
    | Wire of string
    | Signal of int

let parseValue v =
    try
        int v |> Signal
    with _ ->
        Wire v

type Operation =
    | Set of Value * string
    | And of Value * Value * string
    | LeftShift of Value * Value * string
    | Not of Value * string
    | Or of Value * Value * string
    | RightShift of Value * Value * string

let parseOp (op : string) =
    let args = splitBy " " id op
    if args.[0] = "NOT" then Not (parseValue args.[1], args.[3])
    elif args.[1] = "LSHIFT" then LeftShift (parseValue args.[0], parseValue args.[2], args.[4])
    elif args.[1] = "AND" then And (parseValue args.[0], parseValue args.[2], args.[4])
    elif args.[1] = "OR" then Or (parseValue args.[0], parseValue args.[2], args.[4])
    elif args.[1] = "RSHIFT" then RightShift (parseValue args.[0], parseValue args.[2], args.[4])
    elif args.Length = 3 then Set (parseValue args.[0], args.[2])
    else failwithf "%s" op

let parse = parseEachLine parseOp >> Seq.toArray

let applyOp (vals : Map<string, int>) op =
    let getVal v : int option =
        match v with 
        | Signal v' -> Some v'
        | Wire w -> Map.tryFind w vals

    match op with
    | Set (v1, v2) ->
        match getVal v1 with
        | Some v1' -> Map.add v2 v1' vals
        | None -> vals
    | And (v1, v2, v3) -> 
        match (getVal v1, getVal v2) with
        | Some v1', Some v2' -> Map.add v3 (v1' &&& v2') vals
        | _ -> vals
    | Or (v1, v2, v3) -> 
        match (getVal v1, getVal v2) with
        | Some v1', Some v2' -> Map.add v3 (v1' ||| v2') vals
        | _ -> vals
    | Not (v1, v2) -> 
        match getVal v1 with
        | Some v1' -> Map.add v2 (~~~ v1') vals
        | _ -> vals
    | LeftShift (v1, v2, v3) -> 
        match (getVal v1, getVal v2) with
        | Some v1', Some v2' -> Map.add v3 (v1' <<< v2') vals
        | _ -> vals
    | RightShift (v1, v2, v3) -> 
        match (getVal v1, getVal v2) with
        | Some v1', Some v2' -> Map.add v3 (v1' >>> v2') vals
        | _ -> vals

let solvePart1 input =
    let rec keepTrying vals =
        if Map.containsKey "a" vals then vals.["a"]
        else
            input
            |> Seq.fold applyOp vals
            |> keepTrying

    keepTrying Map.empty

let solvePart2 input =
    let newB = solvePart1 input
    let newInput = Array.append input [| Set (Signal newB, "b")|]

    let rec keepTrying vals =
        if Map.containsKey "a" vals then vals.["a"]
        else
            newInput
            |> Seq.fold applyOp vals
            |> keepTrying

    keepTrying Map.empty

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }