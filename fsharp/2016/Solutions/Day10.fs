module Year2016Day10

open AdventOfCode.FSharp.Common

type Receiver = Output | Bot
let parseReceiver = function "output" -> Output | _ -> Bot
type Action = Give of int * Receiver * int * Receiver * int | Go of int * int

let asAction line =
    let ints = extractInts line
    if Array.length ints = 3 then
        let parts = splitBy " " asStringArray line
        let r1 = parseReceiver parts.[5]
        let r2 = parseReceiver parts.[10]
        Give (ints.[0], r1, ints.[1], r2, ints.[2])
    else
        Go (ints.[0], ints.[1])

type Bot = {id: int; chips: (int option) * (int option); giveLow: (Receiver * int) option; giveHigh: (Receiver * int) option}

let parse = parseEachLine asAction

let createBot id = {id=id; chips=None, None; giveLow=None; giveHigh=None}

let giveChip value recvType botId botMap =
    let bot = 
        match Map.tryFind (recvType, botId) botMap with
        | Some b -> b
        | None -> createBot botId
    let newChips =
        match bot.chips with
        | (None, None) -> (Some (value), None)
        | (Some b, None) when b < value -> (Some b, Some value)
        | (Some b, None) -> (Some value, Some b)
        | c -> c
    let bot = {bot with chips=newChips}
    Map.add (recvType, botId) bot botMap

let updateBotMap botMap action =
    match action with
    | Give (botId, r1, t1, r2, t2) ->
        let bot = 
            match Map.tryFind (Bot, botId) botMap with
            | Some b -> b
            | None -> createBot botId
        let bot = {bot with giveLow=Some(r1, t1); giveHigh=Some(r2, t2)}
        Map.add (Bot, botId) bot botMap
    | Go (value, botId) -> giveChip value Bot botId botMap

let giveToBots botMap botId =
    let bot = Map.find (Bot, botId) botMap
    let receiverType1, id1 = bot.giveLow.Value
    let receiverType2, id2 = bot.giveHigh.Value
    let v1, v2 = bot.chips
    botMap
    |> giveChip v1.Value receiverType1 id1
    |> giveChip v2.Value receiverType2 id2

let solvePart1 lines =
    let botMap = Seq.fold updateBotMap Map.empty lines
    let unchecked = botMap |> Map.toSeq |> Seq.map (fst >> snd) |> Seq.toList
    let readyToGive botMap i = 
        let bot = Map.find (Bot, i) botMap
        let c1, c2 = bot.chips
        c1.IsSome && c2.IsSome
    let rec parseUnchecked (unchecked : int list) botMap =
        match unchecked with
        | [] -> botMap
        | _ ->
            let ready, notReady = unchecked |> List.partition (readyToGive botMap)
            let newMap = List.fold giveToBots botMap ready
            parseUnchecked notReady newMap
    let finalMap = parseUnchecked unchecked botMap
    finalMap
    |> Map.toSeq
    |> Seq.find (fun (k, v) -> 
        let c1, c2 = v.chips
        c1.IsSome && c2.IsSome && c1.Value = 17 && c2.Value = 61 )
    |> fst
    |> snd

let solvePart2 lines = 
    let botMap = Seq.fold updateBotMap Map.empty lines
    let unchecked = botMap |> Map.toSeq |> Seq.map (fst >> snd) |> Seq.toList
    let readyToGive botMap i = 
        let bot = Map.find (Bot, i) botMap
        let c1, c2 = bot.chips
        c1.IsSome && c2.IsSome
    let rec parseUnchecked (unchecked : int list) botMap =
        match unchecked with
        | [] -> botMap
        | _ ->
            let ready, notReady = unchecked |> List.partition (readyToGive botMap)
            let newMap = List.fold giveToBots botMap ready
            parseUnchecked notReady newMap
    let finalMap = parseUnchecked unchecked botMap
    let b1 = Map.find (Output, 0) finalMap
    let b2 = Map.find (Output, 1) finalMap
    let b3 = Map.find (Output, 2) finalMap
    (fst b1.chips).Value * (fst b2.chips).Value * (fst b3.chips).Value

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }