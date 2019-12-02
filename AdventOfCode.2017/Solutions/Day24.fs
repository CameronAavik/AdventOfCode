module Year2017Day24

open CameronAavik.AdventOfCode.Common

let asComponent = splitBy "/" asIntArray >> (fun a -> a.[0], a.[1])
let strength = List.sumBy (fun c -> fst c + snd c)
let rec build bridge next components =
    seq { 
        yield bridge
        // if we have a component which bridges to itself that can connect, then always use this. This cuts down a lot of branches in the DFS
        if Set.contains (next, next) components then yield! build ((next, next) :: bridge) next (Set.remove (next, next) components)
        else
            let bridgeable = Set.filter (fun c -> fst c = next || snd c = next) components
            for comp in bridgeable do
                let next' = if snd comp = next then fst comp else snd comp
                yield! build (comp :: bridge) next' (Set.remove comp components) }
let solve maximiser = set >> build [] 0 >> Seq.maxBy maximiser >> strength
let solver = {parse = parseEachLine asComponent; part1 = solve strength; part2 = solve (fun c -> (List.length c, strength c))}