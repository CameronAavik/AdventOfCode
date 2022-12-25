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
