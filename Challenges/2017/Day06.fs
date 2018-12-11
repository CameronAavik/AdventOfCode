module Year2017Day06

open CameronAavik.AdventOfCode.Common

let banksToStr = Array.map (fun i -> i.ToString()) >> String.concat ","

let distribute numBanks maxV maxI i v =
    let doesOverlap = ((if i <= maxI then numBanks else 0) + i) <= (maxI + (maxV % numBanks))
    (if i = maxI then 0 else v) + (maxV / numBanks) + (if doesOverlap then 1 else 0)
    
let solve = 
    let rec solve' seen c banks = 
        if (Map.containsKey (banksToStr banks) seen) then (seen, banks)
        else
            let maxV = Array.max banks
            let maxI = Array.findIndex ((=) maxV) banks
            solve' (Map.add (banksToStr banks) c seen) (c + 1) (Array.mapi (distribute (Seq.length banks) maxV maxI) banks)
    solve' Map.empty 0

let part1 = solve >> fst >> Map.count
let part2 = solve >> (fun (seen, banks) -> (Map.count seen) - (Map.find (banksToStr banks) seen))
let solver = {parse = parseFirstLine (splitBy "\t" asIntArray); part1 = part1; part2 = part2}