using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Y2022.Solvers;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

[GenericTypeArguments(typeof(Day01))]
[GenericTypeArguments(typeof(Day02))]
[GenericTypeArguments(typeof(Day03))]
[GenericTypeArguments(typeof(Day04))]
[GenericTypeArguments(typeof(Day05))]
[GenericTypeArguments(typeof(Day06))]
[GenericTypeArguments(typeof(Day07))]
[GenericTypeArguments(typeof(Day19))]
[GenericTypeArguments(typeof(Day20))]
public class Y2022Solver<TSolver> : SolverBenchmarkBase<TSolver> where TSolver : ISolver
{
}
