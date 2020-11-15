using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Configs;
using Perfolizer.Horology;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using System.Reflection;

namespace AdventOfCode.CSharp.Benchmarks
{
    public record Problem(int Year, int Day)
    {
        public override string? ToString() => $"{Year}.{Day:D2}";
    }

    public class Solvers
    {
        [ParamsSource(nameof(Problems))]
        public Problem Problem = default!;

        private ISolver _solver = default!;
        private ReadOnlyMemory<char> _fileBytes = default!;

        public static IEnumerable<Problem> Problems()
        {
            yield return new Problem(2016, 1);
            //for (int year = 2015; year <= 2019; year++)
            //{
            //    for (int day = 1; day <= 25; day++)
            //    {
            //        if (GetSolverType(year, day) != null)
            //        {
            //            yield return new Problem(year, day);
            //        }
            //    }
            //}
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _solver = (ISolver)Activator.CreateInstance(GetSolverType(Problem.Year, Problem.Day)!)!;
            _fileBytes = File.ReadAllText($"input/{Problem.Year}/day{Problem.Day:D2}.txt").AsMemory();
        }

        [Benchmark]
        public Solution Solve() => _solver.Solve(_fileBytes.Span);

        private static Type? GetSolverType(int year, int day)
        {
            Assembly? assembly = year switch
            {
                2015 => typeof(Y2015.Solvers.Day01).Assembly,
                2016 => typeof(Y2016.Solvers.Day01).Assembly,
                2017 => typeof(Y2017.Solvers.Day01).Assembly,
                2018 => typeof(Y2018.Solvers.Day01).Assembly,
                2019 => typeof(Y2019.Solvers.Day01).Assembly,
                _ => null
            };

            if (assembly != null)
            {
                foreach (Type t in assembly.GetTypes())
                {
                    if (t.Name == $"Day{day:D2}")
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
        private static readonly IConfig s_config = DefaultConfig.Instance
            .AddJob(Job.Default
                .WithWarmupCount(1)
                .WithIterationTime(TimeInterval.FromMilliseconds(250)))
            .AddColumn(StatisticColumn.Median, StatisticColumn.Min, StatisticColumn.Max)
            .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Microsecond));

        static void Main() => BenchmarkRunner.Run<Solvers>(s_config);
    }
}
