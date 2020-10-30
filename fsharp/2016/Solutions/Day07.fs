module Year2016Day07

open AdventOfCode.FSharp.Common

let parse = parseEachLine asString

let rec getBabs babs chars = 
    match chars with
    | a :: b :: c :: cs ->
        if a = c && b <> a then
            getBabs ((string a + string b + string c) :: babs) (b :: c:: cs)
        else
            getBabs babs (b :: c:: cs)
    | _ -> babs

let rec isAbba chars = 
    match chars with
    | a :: b :: c :: d :: cs ->
        if a = d && b = c && a <> b then
            true
        else
            isAbba (b :: c :: d :: cs)
    | _ -> false

let rec parseIP (ip : string) hasA =
    if ip.StartsWith("[") then
        let eb = ip.IndexOf("]")
        let subGroup = ip.[1..eb-1]
        let isA = isAbba (subGroup |> Seq.toList)
        if isA then
            false
        else
            parseIP ip.[eb+1..] hasA
    else
        let fb = ip.IndexOf("[")
        if fb = -1 then
            hasA || isAbba (ip |> Seq.toList)
        else
            let subGroup = ip.[..fb-1]
            let isA = hasA || isAbba (subGroup |> Seq.toList)
            parseIP ip.[fb..] isA

let solvePart1 lines =
    lines |> Seq.filter (fun ip -> parseIP ip false) |> Seq.length

let parseIP' (ip : string) =
    let rec parseIP2 (ip : string) outerBabs innerBabs =
        if ip.StartsWith("[") then
            let eb = ip.IndexOf("]")
            let subGroup = ip.[1..eb-1] |> Seq.toList
            let innerBabs = (getBabs [] subGroup) @ innerBabs
            parseIP2 ip.[eb+1..] outerBabs innerBabs
        else
            let fb = ip.IndexOf("[")
            let subGroup = if fb = -1 then ip else ip.[..fb-1]
            let subGroupLst = subGroup |> Seq.toList
            let outerBabs = (getBabs [] subGroupLst) @ outerBabs
            if fb = -1 then
                innerBabs, outerBabs
            else
                parseIP2 ip.[fb..] outerBabs innerBabs
    parseIP2 ip [] []

let isOpposite bab aba =
    let charsA = bab |> Seq.toArray
    let charsB = aba |> Seq.toArray
    charsA.[0] = charsB.[1] && charsA.[1] = charsB.[0]

let hasSsl ip =
    let inners, outers = parseIP' ip
    inners
    |> Seq.exists (fun i -> Seq.exists (isOpposite i) outers)
    

let solvePart2 lines =
    lines |> Seq.filter hasSsl|> Seq.length

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }