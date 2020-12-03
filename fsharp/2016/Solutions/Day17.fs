module Year2016Day17

open AdventOfCode.FSharp.Common
open System.Security.Cryptography
open System.Text

let getHash (input : string) =
    let md5Obj = MD5.Create()
    let inputAsBytes = Encoding.ASCII.GetBytes input
    let getHash' (path : byte []) =
        let byteArr = Array.append inputAsBytes path
        md5Obj.ComputeHash byteArr
    getHash'

let parse = parseFirstLine asString

type Path = 
    { Path : byte [] ; X : int; Y : int }
    static member empty = { Path = [| |]; X = 0; Y = 0; }
    member path.NextSteps (hashF) =
        let hash : byte [] = hashF path.Path
        let directions = 
            seq {
                if path.Y > 0 && (hash.[0] >>> 4 > 10uy) then (byte 'U', path.X, path.Y - 1)
                if path.Y < 3 && (hash.[0] &&& 0xFuy > 10uy) then (byte 'D', path.X, path.Y + 1)
                if path.X > 0 && (hash.[1] >>> 4 > 10uy) then (byte 'L', path.X - 1, path.Y)
                if path.X < 3 && (hash.[1] &&& 0xFuy > 10uy)  then (byte 'R', path.X + 1, path.Y) }
        directions
        |> Seq.map (fun (dir, x, y) -> { Path = Array.append path.Path [| dir |]; X = x; Y = y })
        |> Seq.toArray


let solveShortest width height input =
    let hashF = getHash input
    let rec bfsStep (fringe : Path array) =
        let nextFringe = fringe |> Array.collect (fun p -> p.NextSteps hashF)
        match (nextFringe |> Array.tryFind (fun p -> p.X = width - 1 && p.Y = height - 1)) with
        | Some path -> path.Path |> Seq.map char |> charsToStr
        | None -> bfsStep nextFringe
    bfsStep [| Path.empty |]

let solveLongest width height input =
    let hashF = getHash input
    let rec bfsStep (fringe : Path array) (longest : Path option) =
        let nextFringe = fringe |> Array.collect (fun p -> p.NextSteps hashF)
        let atEnd, notAtEnd = Array.partition (fun p -> p.X = width - 1 && p.Y = height - 1) nextFringe
        if notAtEnd.Length = 0 then
            let longestPath =
                if atEnd.Length = 0 then longest.Value
                else atEnd.[0]
            longestPath.Path |> Seq.map char |> charsToStr
        elif atEnd.Length = 0 then bfsStep notAtEnd longest
        else bfsStep notAtEnd (Some atEnd.[0])
    bfsStep [| Path.empty |] None

let solvePart1 input = solveShortest 4 4 input

let solvePart2 input = solveLongest 4 4 input |> String.length

let solver = { parse = parse; part1 = solvePart1; part2 = solvePart2 }