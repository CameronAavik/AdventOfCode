using System.Collections.Generic;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

//[BenchmarkDotNet.Diagnostics.Windows.Configs.EtwProfiler]
public class Benchmarks
{
    private readonly ISolver[,] _solvers = new ISolver[7, 25];
    private readonly string?[,] _inputs = new string[7, 25];

    private readonly char[] _part1Buffer = new char[128];
    private readonly char[] _part2Buffer = new char[128];

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
    public Solution SolveYear(int year)
    {
        var solution = new Solution(_part1Buffer, _part2Buffer);
        for (int day = 0; day < 25; day++)
        {
            ISolver solver = _solvers[year - 2015, day - 1];
            string input = _inputs[year - 2015, day - 1]!;
            solver.Solve(input, solution);
        }

        return solution;
    }

    [Benchmark]
    [Arguments(2015, 25)]
    [Arguments(2021, 19)]
    //[ArgumentsSource(nameof(AllDays))]
    public void Solve(int year, int day)
    {
        ISolver solver = _solvers[year - 2015, day - 1];
        string input = _inputs[year - 2015, day - 1]!;
        var solution = new Solution(_part1Buffer, _part2Buffer);
        solver.Solve(input, solution);
    }
}
