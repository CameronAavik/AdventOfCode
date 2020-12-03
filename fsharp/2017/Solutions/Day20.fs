module Year2017Day20

open AdventOfCode.FSharp.Common

let asVector (s : string) = splitBy "," (Array.map int64) s.[3..(s.Length-2)] |> (fun v -> (v.[0],v.[1],v.[2]))
let asParticle = splitBy ", " (Array.map asVector >> (fun vecs -> (vecs.[0],vecs.[1],vecs.[2])))
    
let addVec (x1,y1,z1) (x2,y2,z2) = (x1 + x2, y1 + y2, z1 + z2)
let tickParticle = (fun (p, v, a) -> (p, addVec v a, a)) >> (fun (p, v, a) -> (addVec p v, v, a))
let dist (x, y, z) = abs x + abs y + abs z
    
let ticksToSimulate = 200
let solve transformList = 
    let tickAll = List.map (fun (pos, p) -> (pos, tickParticle p)) >> transformList
    let rec tick t particles = if t = ticksToSimulate then particles else tick (t + 1) (tickAll particles)
    List.mapi (fun i v -> i, v) >> tick 0
    
let filterColliding = List.groupBy (fun (_, (p, _, _)) -> p) >> List.filter (fun (_, l) -> List.tail l = []) >> List.collect snd
let part1 = solve id >> List.minBy (fun (_, (p, v, a)) -> (dist a, dist v, dist p)) >> fst
let part2 = solve filterColliding >> List.length

let solver = {parse = parseEachLine asParticle >> Seq.toList; part1 = part1; part2 = part2}