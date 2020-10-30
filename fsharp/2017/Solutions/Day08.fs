module Year2017Day08

open AdventOfCode.FSharp.Common

let parseIncOrDec = function 
    | "inc" -> (+)
    | "dec" -> (-)
    | _ -> (fun _ x -> x)

let parseOperator = function 
    | ">" -> (>) 
    | "<" -> (<)
    | ">=" -> (>=)
    | "<=" -> (<=)
    | "==" -> (=)
    | "!=" -> (<>)
    | _ -> (fun _ _ -> false)

let asInstruction line = 
    let parts = splitBy " " asStringArray line
    let incOrDecFunc = fun x -> (parseIncOrDec parts.[1]) x (int parts.[2])
    let operatorFunc = fun x -> (parseOperator parts.[5]) x (int parts.[6])
    parts.[0], incOrDecFunc, parts.[4], operatorFunc

let getOrDefault key map ``default``= 
    match Map.tryFind key map with
    | Some v -> v
    | None -> ``default``

let simulate (var1, incOrDec, var2, passesComparisonCheck) vars = 
    let val1, val2 = getOrDefault var1 vars 0, getOrDefault var2 vars 0
    if passesComparisonCheck val2 then Map.add var1 (incOrDec val1) vars else vars
let maxVar = Map.toSeq >> Seq.map snd >> Seq.max
    
let solve = Seq.mapFold (fun vars insn -> simulate insn vars |> (fun v -> (maxVar v, v))) Map.empty >> fst
let solver = {parse = parseEachLine asInstruction; part1 = solve >> Seq.last; part2 = solve >> Seq.max}