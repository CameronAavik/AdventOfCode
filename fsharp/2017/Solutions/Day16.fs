module Year2017Day16

open AdventOfCode.FSharp.Common
open System

type DanceMove = Spin of int | Exchange of int * int | Partner of int * int
    
let asMove (move : string) = 
    match move.[0] with
    | 's' -> Spin (move.[1..] |> int)
    | 'x' -> Exchange (move.[1..] |> splitBy "/" (fun x -> (int x.[0], int x.[1])))
    | 'p' -> Partner (int move.[1] - int 'a', int move.[3] - int 'a')
    | _ -> Spin 0

let swap (i, j) list = list |> List.mapi (fun k h -> if k = i then list.[j] elif k = j then list.[i] else h)
let performMove order = function
    | Spin i -> (List.skip (16 - i) order) @ (List.take (16-i) order)
    | Exchange (a, b) -> swap (a, b) order
    | Partner (a, b) -> swap ((List.findIndex ((=) a) order), (List.findIndex ((=)b) order)) order
    
let orderToStr = List.map ((+) (int 'a') >> char) >> String.Concat
let performNDances n moves =
    let performDance order = Array.fold performMove order moves
    let rec performNDances' dances order = function
        | 0 -> orderToStr order
        | x when List.contains (orderToStr order) dances -> List.item (n % (n - x)) dances
        | x -> performNDances' (dances @ [orderToStr order]) (performDance order) (x - 1)
    performNDances' List.empty [0..15] n
    
let solver = {parse = parseFirstLine (splitBy "," (Array.map asMove)); part1 = performNDances 1; part2 = performNDances 1_000_000_000}