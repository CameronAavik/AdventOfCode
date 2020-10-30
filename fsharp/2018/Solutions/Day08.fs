module Year2018Day08

open AdventOfCode.FSharp.Common

let solve getValue =
    let rec getTree tree =
        let subChildren, metadata = List.item 0 tree, List.item 1 tree
        let subValues, tree' = List.mapFold (fun t _ -> getTree t) (List.skip 2 tree) [1..subChildren]
        let meta, remainingTree = List.splitAt metadata tree'
        getValue meta subValues, remainingTree
    Array.toList >> getTree >> fst
        
let part1Value meta subValues = (List.sum meta) + (List.sum subValues)
let part2Value meta subValues =
    let getSubtotal i = 
        match List.tryItem (i - 1) subValues with
        | Some subtotal -> subtotal
        | None -> 0
    List.sumBy (if List.isEmpty subValues then id else getSubtotal) meta

let solver = {parse = parseFirstLine (splitBy " " asIntArray); part1 = solve part1Value; part2 = solve part2Value}