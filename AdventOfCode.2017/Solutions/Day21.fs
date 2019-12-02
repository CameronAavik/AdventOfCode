module Year2017Day21

open CameronAavik.AdventOfCode.Common

let maxIndex p = Array2D.length1 p - 1
let flatten grid = seq {for x in [0..maxIndex grid] do
                            for y in [0..maxIndex grid] do 
                                yield grid.[x, y]}
let asRule = splitBy " => " (Array.map (splitBy "/" array2D) >> (fun arr -> (arr.[0], arr.[1])))
let gridToStr = flatten >> Seq.fold (fun str x -> str + x.ToString()) ""
let genPerms (pattern, out) =
    let flip (p : 'a [,]) = Array2D.mapi (fun x y _ -> p.[x, maxIndex p - y]) p
    let rotate (p : 'a [,]) = Array2D.mapi (fun x y _ -> p.[maxIndex p - y, x]) p
    let rec gen p = seq { yield p; yield (flip p); yield! gen (rotate p)}
    gen pattern |> Seq.take 8 |> Seq.map (fun p -> (gridToStr p, out))
    
let iterate grid (rules : Map<string, char[,]>) =
    let currentGridSize = maxIndex grid + 1
    let subSize = if (currentGridSize % 2) = 0 then 2 else 3
    let size = currentGridSize / subSize
    let subgrids = seq {for x in [0..size-1] do 
                            for y in [0..size-1] do 
                                yield grid.[subSize*x .. subSize*(x+1)-1, subSize*y .. subSize*(y+1)-1]}
    let enhancedSubgrids =
        subgrids
        |> Seq.map gridToStr
        |> Seq.map (fun subGridStr -> Map.find subGridStr rules)
        |> Seq.toArray
    let subSize' = subSize + 1
    Array2D.init (subSize' * size) (subSize' * size) (fun x y -> enhancedSubgrids.[(x/subSize' * size) + (y/subSize')].[x%subSize',y%subSize'])
            
let getActiveCount = flatten >> Seq.filter (fun c -> c = '#') >> Seq.length
let solve iterations rules =
    let rec getIterations grid = seq { yield grid; yield! getIterations (iterate grid rules)}
    getIterations (array2D [".#.";"..#";"###"]) |> Seq.item iterations |> getActiveCount
        
let solver = {parse = parseEachLine asRule >> Seq.collect genPerms >> Map.ofSeq; part1 = solve 5; part2 = solve 18}