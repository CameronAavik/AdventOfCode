using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Y2023.Solvers;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

[GenericTypeArguments(typeof(Day01))]
public class Y2023Solver<TSolver> : SolverBenchmarkBase<TSolver> where TSolver : ISolver
{
}
