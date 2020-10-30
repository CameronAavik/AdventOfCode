module Year2017Day22

open System.Collections.Generic
open AdventOfCode.FSharp.Common

// tuples are slow when used as a map/dictionary key for some reason, convert 
// the coord to a long instead. Value determined by n^2+n=Int64.MaxValue
let toHash (x, y) = x + 3037000500L * y
let toGridMap lines = 
    // Map was too slow, we use a mutable Dictionary instead
    let grid = new Dictionary<int64, int>();
    let center = (String.length (Seq.head lines)) / 2
    let defaultEntries = 
        lines 
        |> Seq.mapi (fun i r -> Seq.mapi (fun j c -> ((j-center, i-center), if c = '#' then 2 else 0)) r) 
        |> Seq.collect id
    for (x, y), v in defaultEntries do
        grid.[toHash (int64 x, int64 y)] <- v
    grid
    
let move (x, y) = function 0 -> (x, y - 1L) | 1 -> (x + 1L, y) | 2 -> (x, y + 1L) | 3 -> (x - 1L, y) | _ -> (x, y)
let solve jump iterations (grid : Dictionary<int64, int>) = 
    let rec step pos dir infected = function
        | 0 -> infected
        | n ->
            let node = grid.GetValueOrDefault(toHash pos, 0)
            let dir', node' = (dir + node + 3) % 4, (node + jump) % 4
            grid.[toHash pos] <- node'
            step (move pos dir') dir' (infected + if node' = 2 then 1 else 0) (n - 1)
    step (0L, 0L) 0 0 iterations

let solver = {parse = parseEachLine asString >> toGridMap; part1 = solve 2 10000; part2 = solve 1 10000000}