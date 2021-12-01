using System.Collections.Generic;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

public class Benchmarks
{
    private readonly ISolver[,] _solvers = new ISolver[7, 25];
    private readonly string?[,] _inputs = new string[7, 25];

    public static IEnumerable<object[]> AllDays()
    {
        for (int year = 2015; year <= 2021; year++)
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
            _inputs[year - 2015, day - 1] = AdventRunner.GetInputAsync(year, day).GetAwaiter().GetResult();
        }
    }

    //[Benchmark]
    [Arguments(2015)]
    [Arguments(2020)]
    public Solution[] SolveYear(int year)
    {
        Solution[] solutions = new Solution[25];
        for (int day = 0; day < 25; day++)
            solutions[day] = _solvers[year - 2015, day].Solve(_inputs[year - 2015, day]);
        return solutions;
    }

    [Benchmark]
    [Arguments(2021, 1)]
    //[ArgumentsSource(nameof(AllDays))]
    public Solution Solve(int year, int day) =>
        _solvers[year - 2015, day - 1].Solve(_inputs[year - 2015, day - 1]);
}
