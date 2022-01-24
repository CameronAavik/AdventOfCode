using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

//[BenchmarkDotNet.Diagnostics.Windows.Configs.EtwProfiler]
[MemoryDiagnoser(displayGenColumns: false)]
public abstract class BenchmarkBase
{
    private readonly ISolver _solver;
    private readonly byte[] _input;
    private readonly char[] _part1Buffer = new char[128];
    private readonly char[] _part2Buffer = new char[128];

    public BenchmarkBase(int year, int day)
    {
        _solver = AdventRunner.GetSolver(year, day)!;
        _input = AdventRunner.GetInputAsync(year, day).GetAwaiter().GetResult();
    }

    [Benchmark]
    public void Solve() => _solver.Solve(_input, new(_part1Buffer, _part2Buffer));
}
