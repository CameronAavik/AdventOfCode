using System.Collections.Generic;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Perfolizer.Horology;

namespace AdventOfCode.CSharp.Benchmarks
{
    public class Benchmarks
    {
        private readonly ISolver[,] _solvers = new ISolver[6, 25];
        private readonly string?[,] _inputs = new string[6, 25];

        public static IEnumerable<object[]> AllDays()
        {
            for (int year = 2015; year <= 2020; year++)
            {
                for (int day = 1; day <= 25; day++)
                {
                    if (AdventRunner.GetSolverType(year, day) != null)
                    {
                        yield return new object[] { year, day };
                    }
                }
            }
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            foreach (object[]? days in AllDays())
            {
                int year = (int)days[0];
                int day = (int)days[1];
                _solvers[year - 2015, day - 1] = AdventRunner.GetSolver(year, day)!;
                _inputs[year - 2015, day - 1] = AdventRunner.GetInput(year, day);
            }
        }

        [Benchmark]
        //[Arguments(2020, 9)]
        [ArgumentsSource(nameof(AllDays))]
        public Solution Solve(int year, int day) =>
            _solvers[year - 2015, day - 1].Solve(_inputs[year - 2015, day - 1]);
    }

    public class Program
    {
        static void Main() => BenchmarkRunner.Run<Benchmarks>(Config);

        private static IConfig Config => DefaultConfig.Instance
            .AddJob(Job.Default)
            .WithSummaryStyle(SummaryStyle.Default.WithTimeUnit(TimeUnit.Microsecond))
            .ClearColumns()
            .AddColumnProvider(DefaultColumnProviders.Descriptor)
            .AddColumnProvider(DefaultColumnProviders.Params)
            .AddColumn(StatisticColumn.P0)
            .AddColumn(StatisticColumn.P50)
            .AddColumn(StatisticColumn.P100);
    }
}
