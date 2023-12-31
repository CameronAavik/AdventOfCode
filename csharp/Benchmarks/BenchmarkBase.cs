using System;
using System.IO;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

//[BenchmarkDotNet.Diagnostics.Windows.Configs.EtwProfiler]
[MemoryDiagnoser(displayGenColumns: false)]
public abstract class SolverBenchmarkBase<TSolver> where TSolver : ISolver
{
    private readonly byte[] _input;
    private readonly char[] _part1Buffer = new char[128];
    private readonly char[] _part2Buffer = new char[128];

    public SolverBenchmarkBase()
    {
        (int year, int day) = SolverUtils.GetYearAndDay<TSolver>();
        _input = AdventRunner.GetInputAsync(year, day).GetAwaiter().GetResult();
    }

    [Benchmark]
    public void Solve() => TSolver.Solve(_input, new(_part1Buffer, _part2Buffer));
}

//[BenchmarkDotNet.Diagnostics.Windows.Configs.EtwProfiler]
[MemoryDiagnoser(displayGenColumns: false)]
public abstract class MultiInputSolverBenchmarkBase<TSolver> where TSolver : ISolver
{
    private readonly byte[][] _inputs = new byte[5][];
    private readonly char[] _part1Buffer = new char[128];
    private readonly char[] _part2Buffer = new char[128];

    public MultiInputSolverBenchmarkBase()
    {
        (int year, int day) = SolverUtils.GetYearAndDay<TSolver>();
        var inputFolder = $"input/{year}/extra/day{day:D2}";
        int i = 0;
        foreach (var file in Directory.EnumerateFiles(inputFolder))
            _inputs[i++] = File.ReadAllBytes(file);
    }

    [Benchmark]
    public void SolveAll()
    {
        TSolver.Solve(_inputs[0], new Solution(_part1Buffer, _part2Buffer));
        TSolver.Solve(_inputs[1], new Solution(_part1Buffer, _part2Buffer));
        TSolver.Solve(_inputs[2], new Solution(_part1Buffer, _part2Buffer));
        TSolver.Solve(_inputs[0], new Solution(_part1Buffer, _part2Buffer));
        TSolver.Solve(_inputs[0], new Solution(_part1Buffer, _part2Buffer));
    }

    [Benchmark]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(4)]
    public void SolveOneInput(int n)
    {
        TSolver.Solve(_inputs[n], new Solution(_part1Buffer, _part2Buffer));
    }
}
