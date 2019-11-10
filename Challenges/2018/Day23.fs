module Year2018Day23

open CameronAavik.AdventOfCode.Common
open FSharpx.Collections
open System
open System.IO

type Vec3 =
    {x: int; y: int; z: int}
    static member Zero = {x=0; y=0; z=0}
let corners {x=x; y=y; z=z} radius = 
    [
        {x=x - radius; y=y; z=z}
        {x=x + radius; y=y; z=z}
        {x=x; y=y - radius; z=z}
        {x=x; y=y + radius; z=z}
        {x=x; y=y; z=z - radius}
        {x=x; y=y; z=z + radius}
    ]

let manhattan {x=x0; y=y0; z=z0} {x=x1; y=y1; z=z1} =
    abs (x0 - x1) + abs (y0 - y1) + abs (z0 - z1)

type Octohedron = {pos: Vec3; radius: int}
let doOctohedronsOverlap o1 o2 = manhattan o1.pos o2.pos <= o1.radius + o2.radius
let doesOctohedronContainPoint octo pt = manhattan octo.pos pt <= octo.radius
let doesOctohedronContainAnother o1 o2 = 
    corners o1.pos o1.radius |> List.exists (doesOctohedronContainPoint o2 >> not) |> not
let minManhattanDistanceToOrigin octo = manhattan octo.pos Vec3.Zero - octo.radius

// implemented with reference to 
// https://github.com/sim642/adventofcode/blob/master/src/main/scala/eu/sim642/adventofcode2018/Day23.scala#L91
let divideOctohedron {pos=pos; radius=radius} =
    let offset =
        if radius >= 3 then
            ((float radius) / 3.0) |> floor |> int
        elif radius > 0 then
            1
        else
            0
    let newRadius = radius - offset
    let axisOffsets = corners pos offset
    let offsets = if radius = 1 then pos :: axisOffsets else axisOffsets
    offsets |> List.map (fun p -> {pos=p; radius=newRadius})

type Nanobot = {id: int; pos: Vec3; radius: int}
let asOctohedron bot = {pos=bot.pos; radius=bot.radius}

let asNanobot i line =
    let pos, radius = splitBy ">, " (fun p -> p.[0].[5..], int p.[1].[2..]) line
    let px, py, pz = splitBy "," (fun p -> int p.[0], int p.[1], int p.[2]) pos
    {id=i; pos={x=px; y=py; z=pz}; radius=radius}

let solvePart1 bots =
    let maxRadiusBot = Array.maxBy (fun b -> b.radius) bots
    bots 
    |> Array.filter (fun b -> manhattan b.pos maxRadiusBot.pos <= maxRadiusBot.radius)
    |> Array.length

let getBounds bots =
    let xs = Array.map (fun b -> b.pos.x) bots
    let ys = Array.map (fun b -> b.pos.y) bots
    let zs = Array.map (fun b -> b.pos.z) bots
    Array.min xs, Array.max xs, Array.min ys, Array.max ys, Array.min zs, Array.max zs

let initialOctohedron bots =
    let minX, maxX, minY, maxY, minZ, maxZ = getBounds bots
    let pos = {x=(minX + maxX)/2; y=(minY + maxY)/2; z=(minZ + maxZ)/2}
    let radius = bots |> Array.map (fun b -> manhattan pos b.pos + b.radius) |> Array.max
    {pos=pos; radius=radius}

let getOverlapping octohedron = Array.filter (asOctohedron >> doOctohedronsOverlap octohedron)
let containsAll octohedron = Array.exists (asOctohedron >> doesOctohedronContainAnother octohedron >> not) >> not

// priority is botCount, radius, minManhattanDistance
type QueueItem = {priority: int * int * int; octo: Octohedron; bots: Nanobot []}

let addOctohedronToQueue bots octo =
    let overlapping = getOverlapping octo bots
    let minDist = minManhattanDistanceToOrigin octo
    PriorityQueue.insert {priority=(overlapping.Length, -octo.radius, -minDist); octo=octo; bots=overlapping}

let searchForMin allBots initialOctohedron =
    let rec searchForMin' seen pQueue =
        let {priority=(_, _, negativeMinDist); octo=octo; bots=bots}, pQueue = PriorityQueue.pop pQueue
        if Set.contains octo seen then
            searchForMin' seen pQueue
        else
            let isDone = containsAll octo bots
            if isDone then
                -negativeMinDist
            else
                let seen = seen |> Set.add octo
                let newOctohedrons = divideOctohedron octo
                let pQueue = List.foldBack (addOctohedronToQueue bots) newOctohedrons pQueue
                searchForMin' seen pQueue
    let pQueue = PriorityQueue.empty true |> addOctohedronToQueue allBots initialOctohedron
    searchForMin' Set.empty pQueue

let solvePart2 bots = searchForMin bots (initialOctohedron bots)

let solver = {parse = File.ReadLines >> parseEachLineIndexed asNanobot >> Seq.toArray; part1 = solvePart1; part2 = solvePart2}