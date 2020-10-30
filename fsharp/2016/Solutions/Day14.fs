module Year2016Day14

open AdventOfCode.FSharp.Common
open System.Security.Cryptography
open System.Text
open System.Collections.Generic
open System

let parse = parseFirstLine asString


let getHash (input : string) stretch =
    let md5Obj = MD5.Create()
    let inputAsBytes = Encoding.ASCII.GetBytes input
    let getHash' index =
        let rec hashN i (byteArr : byte []) =
            let hashed = md5Obj.ComputeHash byteArr
            if i <= 1 then hashed
            else
                let asStr = BitConverter.ToString(hashed).Replace("-","").ToLower()
                let asBytes = Encoding.ASCII.GetBytes asStr
                hashN (i - 1) asBytes
        let indexBytes = Encoding.ASCII.GetBytes (string index)
        let byteArr = Array.append inputAsBytes indexBytes
        hashN stretch byteArr
    getHash'
 
let isDouble (b : byte) = (b >>> 4) = (b &&& 0xFuy)

let getFirstTriple (hash : byte array) =
    seq {
        for i in 0 .. (hash.Length - 1) do
            let b = hash.[i]
            if isDouble b then
                let digit = b &&& 0xFuy
                if (i > 0 && (hash.[i - 1] &&& 0xFuy) = digit) || (i < hash.Length - 1 && (hash.[i + 1] >>> 4) = digit) then
                    digit } |> Seq.tryHead

let getPentuples (hash : byte array) =
    seq {
        for i in 0 .. (hash.Length - 2) do
            let b = hash.[i]
            if isDouble b && b = hash.[i + 1] then
                let digit = b &&& 0xFuy
                if (i > 0 && (hash.[i - 1] &&& 0xFuy) = digit) || (i < hash.Length - 2 && (hash.[i + 2] >>> 4) = digit) then
                    digit }

let solve input stretch =
    let getMd5 = getHash input stretch
    
    // Will rewrite to not use mutation later
    let mutable curIndex = 0
    let mutable seenKeys = 0
    let mutable pentupleHistory = [0uy .. 15uy] |> List.map (fun i -> (i, 0)) |> dict |> Dictionary<_, _>
    let mutable lastKey = 0
    
    while seenKeys < 64 do
        let newHash = getMd5 curIndex
        for p in getPentuples newHash do
            pentupleHistory.[p] <- pentupleHistory.[p] + 1
        if curIndex >= 1000 then
            let oldHash = getMd5 (curIndex - 1000)
            for p in getPentuples oldHash do
                pentupleHistory.[p] <- pentupleHistory.[p] - 1
            match getFirstTriple oldHash with
            | Some triple when pentupleHistory.[triple] > 0 ->
                lastKey <- (curIndex - 1000)
                seenKeys <- seenKeys + 1
            | _ -> ()
        curIndex <- curIndex + 1
            
    lastKey

let solvePart1 input = solve input 1
let solvePart2 input = solve input 2017

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }