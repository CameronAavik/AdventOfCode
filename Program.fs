namespace CameronAavik.AdventOfCode

module Program =
    [<EntryPoint>]
    let main argv =
        let runPart year day part = Runner.getSolver year day part true ()
        let runDay year day = for part in 1..2 do runPart year day part
        let runYear year = for day in 1..25 do runDay year day
        match argv.[0] with
            | "BENCH" -> Benchmarking.runBenchmarks()
            | "ALL" -> for year in 2015..2018 do runYear year
            | x ->
                let parts = x.Split('.') |> Array.map int
                match parts.Length with
                | 1 -> runYear parts.[0]
                | 2 -> runDay parts.[0] parts.[1]
                | 3 -> runPart parts.[0] parts.[1] parts.[2]
                | _ -> ()
        0

// Note: I used to use this file as the file that had all my solutions. I have
// since restructured my projects, see the Challenges directory for solutions
// if you found your way here through one of my links somewhere.