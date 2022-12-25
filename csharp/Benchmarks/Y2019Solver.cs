using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Y2019.Solvers;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

[GenericTypeArguments(typeof(Day01))]
[GenericTypeArguments(typeof(Day02))]
public class Y2019Solver<TSolver> : SolverBenchmarkBase<TSolver> where TSolver : ISolver
{
}
