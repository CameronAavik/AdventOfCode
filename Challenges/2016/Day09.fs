module Year2016Day09

open CameronAavik.AdventOfCode.Common
open System

let parse = parseFirstLine asString

let parseGroup (line : char []) i =
    let rb = Array.IndexOf(line.[i..], ')') + i
    let sub = new String(line.[i+1..rb-1])
    let dims = splitBy "x" asIntArray sub
    let w, h = dims.[0], dims.[1]
    let offset = w * h
    rb + (w + 1), offset

let solvePart1 (lines : string) =
    let charArr = lines |> Seq.toArray 
    let rec parseLine i count =
        if i >= charArr.Length then
            count
        else
            let l = charArr.[i]
            if l = '(' then
                let newI, newCount = parseGroup charArr i
                parseLine newI (count + newCount)
            else
                parseLine (i + 1) (count + 1)
    parseLine 0 0

let rec parseGroup2 (line : char []) i =
    let rb = Array.IndexOf(line.[i..], ')') + i
    let sub = new String(line.[i+1..rb-1])
    let dims = splitBy "x" asIntArray sub
    let w, h = dims.[0], int64 dims.[1]

    let rec getCoveredArea ni total =
        if ni >= (rb + w + 1) then
            total
        else
            if line.[ni] = '(' then 
                let newI, subCount, _ = parseGroup2 line ni
                getCoveredArea newI (total + subCount)
            else
                getCoveredArea (ni + 1) (total + 1L)
    let totalArea = getCoveredArea (rb + 1) 0L
    rb + (w + 1), totalArea * h, false

let solvePart2 lines = 
    let charArr = lines |> Seq.toArray 
    let rec parseLine i count isData =
        if i >= charArr.Length then
            count
        else
            let l = charArr.[i]
            if l = '(' && not isData then
                let newI, newCount, isData = parseGroup2 charArr i
                parseLine newI (count + newCount) isData
            else
                parseLine (i + 1) (count + 1L) false
    parseLine 0 0L false

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }