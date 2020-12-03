namespace AdventOfCode.FSharp

open System
open System.IO
open System.Text.RegularExpressions
open System.Collections.Generic

// This is a set of methods that I found myself using many times in my solutions
module Common =
    // Every day has a corresponding Day record which defines how to parse the file
    // then two functions for solving each part respectively
    [<NoComparison>]
    [<NoEquality>]
    type Day<'a, 'b, 'c> = { parse: string -> 'a; part1: 'a -> 'b; part2: 'a -> 'c }

    // helper methods for parsing
    let parseFirstLine f (fileName : string) =
        use fs = new FileStream(fileName, FileMode.Open)
        use reader = new StreamReader(fs)
        f (reader.ReadLine())

    let parseEachLine f = File.ReadLines >> Seq.map f
    let parseEachLineIndexed f = File.ReadLines >> Seq.mapi f

    let asString : string -> string = id
    let asInt : string -> int = int
    let asCharArray (s : string) = s.ToCharArray ()
    let asStringArray : string [] -> string [] = Array.map string
    let asIntArray : string [] -> int [] = Array.map int
    let asInt64Array : string [] -> int64 [] = Array.map int64
    let splitBy (c : string) f (str : string) = str.Split([| c |], StringSplitOptions.None) |> f
    let extractInts str = [| for m in Regex.Matches(str, "(-?\d+)") -> int m.Value |]
    let withRegex regex str = [| for m in Regex.Match(str, regex).Groups -> m.Value|] |> Array.tail

    let charsToStr (chars : char seq) = chars |> Seq.map string |> String.concat ""

    let inline repeatN n f x =
        let rec aux n x =
            if n = 0 then x
            else f x |> aux (n - 1)
        aux n x

    let inline repeatUntil f pred x =
        let rec aux x =
            if pred x then x
            else f x |> aux
        aux x

    let inline repeatUntilDuplicate f x =
        let seen = new HashSet<_>()
        let rec aux x =
            if seen.Contains(x) then x
            else
                seen.Add(x) |> ignore
                f x |> aux
        aux x
