module Year2018Day10

open AdventOfCode.FSharp.Common
open System.Text.RegularExpressions
open System

let asParticle (line : string) =
    let regexPattern = @"position=<\s*(-?\d+),\s*(-?\d+)> velocity=<\s*(-?\d+),\s*(-?\d+)>"
    let m = Regex.Match(line, regexPattern)
    let gs = [ for g in m.Groups -> g.Value ] |> List.tail
    (int gs.[0], int gs.[1]), (int gs.[2], int gs.[3])

let getStartingTime particles =
    // find two particles travelling in opposite direction the fastest
    let (p1x, p1y), (v1x, v1y) = Array.minBy snd particles
    let (p2x, p2y), (v2x, v2y) = Array.maxBy snd particles
    let (pdx, pdy), (vpx, vpy) = (p2x - p1x, p2y - p1y), (v2x - v1x, v2y - v1y)  
    // find time when closest
    double (-vpx * pdx - vpy * pdy) / double (vpx * vpx + vpy * vpy) |> int
        
let particleAtTime t ((px, py), (vx, vy)) = (px + vx * t, py + vy * t)
let particlesAtTime t = Array.map (particleAtTime t)

let getRanges =
    let updateRanges (minX, maxX, minY, maxY) (x, y) =
        min x minX, max x maxX, min y minY, max y maxY
    Array.fold updateRanges (Int32.MaxValue, Int32.MinValue, Int32.MaxValue, Int32.MinValue)

let getBounds particles t =
    let minX, maxX, minY, maxY = particles |> particlesAtTime t |> getRanges
    abs (maxX - minX) + abs (maxY - minY)

let (|?) (a: 'a option) b = if a.IsSome then a.Value else b
        
// take the current time and descend to optimal solution
let rec findBestTime particles startT =
    let rec descend t prev cur next =
        let prev = prev |? getBounds particles (t - 1)
        let cur = cur |? getBounds particles t
        let next = next |? getBounds particles (t + 1)
        if prev < cur then
            descend (t - 1) None (Some prev) (Some cur)
        elif next < cur then
            descend (t + 1) (Some cur) (Some next) None
        else
            t
    descend startT None None None
        
let particlesToMessage particles t =
    let particlesAtTime = particles |> particlesAtTime t
    let minX, maxX, minY, maxY = particlesAtTime |> getRanges
    let particles = particlesAtTime |> Array.map (fun (x, y) -> (x - minX, y - minY)) |> Set.ofArray
    seq {for i in 0 .. maxY - minY do
            yield "\n"
            for j in 0 .. maxX - minX do
                if Set.contains (j, i) particles then
                    yield "█"
                else
                    yield " " } |> String.Concat

let solve particles =
    let particleArray = particles |> Seq.toArray // we are going to be iterating a lot, use an array.
    let startingTime = getStartingTime particleArray
    let bestTime = findBestTime particleArray startingTime
    let message = particlesToMessage particleArray bestTime
    message, bestTime

let solver = {parse = parseEachLine asParticle; part1 = solve >> fst; part2 = solve >> snd}