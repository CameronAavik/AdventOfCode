module Year2017Day10

open AdventOfCode.FSharp.Common

let repeat n folder init = Seq.fold (fun s _ -> folder s) init [0..(n-1)]

let (%!) a b = (a % b + b) % b

type State = {hash: int array; skip: int; start: int}
let revN n s = Array.append (Array.take n s |> Array.rev) (Array.skip n s)
let shift n s = Array.append (Array.skip n s) (Array.take n s)
let step listLen state length = 
    let shiftLen = ((length + state.skip) % listLen)
    let newHash = state.hash |> revN length |> shift shiftLen
    {state with hash = newHash; skip = state.skip + 1; start = (state.start - shiftLen) %! listLen}
    
let solveRound listLen init = Array.fold (step listLen) init
let sparseHash listLen n lengths = 
    repeat n (fun i -> solveRound listLen i lengths) {hash=[|0..(listLen-1)|]; skip=0; start=0} 
    |> (fun s -> shift s.start s.hash)
let denseHash = Array.chunkBySize 16 >> Array.map (Array.fold (^^^) 0)

let strToDenseHash =
    Seq.map int 
    >> Seq.toArray 
    >> (fun hash -> Array.append hash [|17;31;73;47;23|]) 
    >> sparseHash 256 64 
    >> denseHash

let part1 = splitBy "," asIntArray >> sparseHash 256 1 >> (fun s -> s.[0] * s.[1])
let toHexStr = Array.fold (fun h i -> h + sprintf "%02x" i) ""
let solver = {parse = parseFirstLine asString; part1 = part1; part2 = strToDenseHash >> toHexStr}