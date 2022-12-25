using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Y2020.Solvers;
using BenchmarkDotNet.Attributes;

namespace AdventOfCode.CSharp.Benchmarks;

[GenericTypeArguments(typeof(Day01))]
[GenericTypeArguments(typeof(Day02))]
[GenericTypeArguments(typeof(Day03))]
[GenericTypeArguments(typeof(Day04))]
[GenericTypeArguments(typeof(Day05))]
[GenericTypeArguments(typeof(Day06))]
[GenericTypeArguments(typeof(Day07))]
[GenericTypeArguments(typeof(Day08))]
[GenericTypeArguments(typeof(Day09))]
[GenericTypeArguments(typeof(Day10))]
[GenericTypeArguments(typeof(Day11))]
[GenericTypeArguments(typeof(Day12))]
[GenericTypeArguments(typeof(Day13))]
[GenericTypeArguments(typeof(Day14))]
[GenericTypeArguments(typeof(Day15))]
[GenericTypeArguments(typeof(Day16))]
[GenericTypeArguments(typeof(Day17))]
[GenericTypeArguments(typeof(Day18))]
[GenericTypeArguments(typeof(Day19))]
[GenericTypeArguments(typeof(Day20))]
[GenericTypeArguments(typeof(Day21))]
[GenericTypeArguments(typeof(Day22))]
[GenericTypeArguments(typeof(Day23))]
[GenericTypeArguments(typeof(Day24))]
[GenericTypeArguments(typeof(Day25))]
public class Y2020Solver<TSolver> : SolverBenchmarkBase<TSolver> where TSolver : ISolver
{
}
