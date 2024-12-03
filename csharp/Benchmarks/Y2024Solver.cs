using System.IO;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Y2024.Solvers;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

[GenericTypeArguments(typeof(Day01))]
[GenericTypeArguments(typeof(Day02))]
[GenericTypeArguments(typeof(Day03))]
public class Y2024Solver<TSolver> : MultiInputSolverBenchmarkBase<TSolver> where TSolver : ISolver
{
}

[MemoryDiagnoser(displayGenColumns: false)]
public class AllDays2024
{
    private readonly char[] _part1Buffer = new char[128];
    private readonly char[] _part2Buffer = new char[128];
    private readonly byte[][][] _inputs = new byte[25][][];

    public AllDays2024()
    {
        for (int i = 0; i < 25; i++)
        {
            _inputs[i] = new byte[5][];
            string inputFolder = $"input/2024/extra/day{i + 1:D2}";
            if (Directory.Exists(inputFolder))
            {
                int j = 0;
                foreach (string file in Directory.EnumerateFiles(inputFolder))
                    _inputs[i][j++] = File.ReadAllBytes(file);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = 5)]
    public void SolveAllDays()
    {
        for (int i = 0; i < 5; i++)
        {
            Day01.Solve(_inputs[0][i], new(_part1Buffer, _part2Buffer));
            Day02.Solve(_inputs[1][i], new(_part1Buffer, _part2Buffer));
            Day03.Solve(_inputs[2][i], new(_part1Buffer, _part2Buffer));
        }
    }
}
