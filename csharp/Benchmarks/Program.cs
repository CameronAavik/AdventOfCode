using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode.CSharp.Benchmarks
{
    public record Problem(int Year, int Day)
    {
        public override string? ToString() => $"{Year}.{Day:D2}";
    }

    //[MemoryDiagnoser]
    public class Solvers
    {
        [ParamsSource(nameof(Problems))]
        public Problem Problem = default!;

        private ISolver solver = default!;
        private ReadOnlyMemory<char> fileBytes = default!;

        public static IEnumerable<Problem> Problems()
        {
            for (int year = 2015; year <= 2019; year++)
            {
                for (int day = 1; day <= 25; day++)
                {
                    if (GetSolverType(year, day) != null)
                    {
                        yield return new Problem(year, day);
                    }
                }
            }
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            solver = (ISolver)Activator.CreateInstance(GetSolverType(Problem.Year, Problem.Day)!)!;
            fileBytes = File.ReadAllText($"input/{Problem.Year}/day{Problem.Day:D2}.txt").AsMemory();
        }

        [Benchmark]
        public Solution Solve() => solver.Solve(fileBytes.Span);

        private static Type? GetSolverType(int year, int day)
        {
            var assembly = year switch
            {
                2015 => typeof(Y2015.Solvers.Day1).Assembly,
                2016 => typeof(Y2016.Solvers.Day1).Assembly,
                2017 => typeof(Y2017.Solvers.Day1).Assembly,
                2018 => typeof(Y2018.Solvers.Day1).Assembly,
                2019 => typeof(Y2019.Solvers.Day1).Assembly,
                _ => null
            };

            if (assembly != null)
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.Name == $"Day{day}")
                    {
                        return t;
                    }
                }
            }

            return null;
        }
    }

    class Program
    {
        static void Main() => BenchmarkRunner.Run<Solvers>();
    }
}
