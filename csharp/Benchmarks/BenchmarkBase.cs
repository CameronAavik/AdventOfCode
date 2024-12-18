﻿using System.IO;
using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;

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
//[HardwareCounters(HardwareCounter.BranchMispredictions, HardwareCounter.BranchInstructions)]
public abstract class MultiInputSolverBenchmarkBase<TSolver> where TSolver : ISolver
{
    private readonly byte[][] _inputs = new byte[5][];
    private readonly char[] _part1Buffer = new char[128];
    private readonly char[] _part2Buffer = new char[128];

    public MultiInputSolverBenchmarkBase()
    {
        (int year, int day) = SolverUtils.GetYearAndDay<TSolver>();
        string inputFolder = $"input/{year}/extra/day{day:D2}";
        int i = 0;
        // If less than 5 test inputs, might load same file multiple times
        // Useful during early benchmarking
        while (i < 5)
            foreach (string file in Directory.EnumerateFiles(inputFolder))
                while (i < 5)
                    _inputs[i++] = File.ReadAllBytes(file);
    }

    [Benchmark(OperationsPerInvoke = 5)]
    public void SolveAll()
    {
        TSolver.Solve(_inputs[0], new Solution(_part1Buffer, _part2Buffer));
        TSolver.Solve(_inputs[1], new Solution(_part1Buffer, _part2Buffer));
        TSolver.Solve(_inputs[2], new Solution(_part1Buffer, _part2Buffer));
        TSolver.Solve(_inputs[3], new Solution(_part1Buffer, _part2Buffer));
        TSolver.Solve(_inputs[4], new Solution(_part1Buffer, _part2Buffer));
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
