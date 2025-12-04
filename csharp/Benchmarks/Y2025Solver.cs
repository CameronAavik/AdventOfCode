using System.IO;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Y2025.Solvers;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

[GenericTypeArguments(typeof(Day01))]
[GenericTypeArguments(typeof(Day02))]
[GenericTypeArguments(typeof(Day03))]
[GenericTypeArguments(typeof(Day04))]
public class Y2025Solver<TSolver> : MultiInputSolverBenchmarkBase<TSolver> where TSolver : ISolver
{
}

[MemoryDiagnoser(displayGenColumns: false)]
public class AllDays2025
{
    private readonly char[] _part1Buffer = new char[128];
    private readonly char[] _part2Buffer = new char[128];
    private readonly byte[][][] _inputs = new byte[25][][];

    public AllDays2025()
    {
        for (var i = 0; i < 25; i++)
        {
            _inputs[i] = new byte[5][];
            var inputFolder = $"input/2025/extra/day{i + 1:D2}";
            if (Directory.Exists(inputFolder))
            {
                var j = 0;
                foreach (var file in Directory.EnumerateFiles(inputFolder))
                    _inputs[i][j++] = File.ReadAllBytes(file);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = 5)]
    public void SolveAllDays()
    {
        for (var i = 0; i < 5; i++)
        {
            Day01.Solve(_inputs[0][i], new(_part1Buffer, _part2Buffer));
            Day02.Solve(_inputs[1][i], new(_part1Buffer, _part2Buffer));
            Day03.Solve(_inputs[1][i], new(_part1Buffer, _part2Buffer));
            Day04.Solve(_inputs[1][i], new(_part1Buffer, _part2Buffer));
        }
    }
}
