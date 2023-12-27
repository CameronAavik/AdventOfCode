using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Runner;
using AdventOfCode.CSharp.Y2023.Solvers;
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
public class Y2023Solver<TSolver> : SolverBenchmarkBase<TSolver> where TSolver : ISolver
{
}

[MemoryDiagnoser(displayGenColumns: false)]
public class AllDays2023
{
    private readonly char[] _part1Buffer = new char[128];
    private readonly char[] _part2Buffer = new char[128];
    private readonly byte[][] _inputs = new byte[25][];

    public AllDays2023()
    {
        for (int i = 0; i < 25; i++)
            _inputs[i] = AdventRunner.GetInputAsync(2023, i + 1).GetAwaiter().GetResult();
    }

    [Benchmark]
    public void SolveAllDays()
    {
        Day01.Solve(_inputs[0], new(_part1Buffer, _part2Buffer));
        Day02.Solve(_inputs[1], new(_part1Buffer, _part2Buffer));
        Day03.Solve(_inputs[2], new(_part1Buffer, _part2Buffer));
        Day04.Solve(_inputs[3], new(_part1Buffer, _part2Buffer));
        Day05.Solve(_inputs[4], new(_part1Buffer, _part2Buffer));
        Day06.Solve(_inputs[5], new(_part1Buffer, _part2Buffer));
        Day07.Solve(_inputs[6], new(_part1Buffer, _part2Buffer));
        Day08.Solve(_inputs[7], new(_part1Buffer, _part2Buffer));
        Day09.Solve(_inputs[8], new(_part1Buffer, _part2Buffer));
        Day10.Solve(_inputs[9], new(_part1Buffer, _part2Buffer));
        Day11.Solve(_inputs[10], new(_part1Buffer, _part2Buffer));
        Day12.Solve(_inputs[11], new(_part1Buffer, _part2Buffer));
        Day13.Solve(_inputs[12], new(_part1Buffer, _part2Buffer));
        Day14.Solve(_inputs[13], new(_part1Buffer, _part2Buffer));
        Day15.Solve(_inputs[14], new(_part1Buffer, _part2Buffer));
        Day16.Solve(_inputs[15], new(_part1Buffer, _part2Buffer));
        Day17.Solve(_inputs[16], new(_part1Buffer, _part2Buffer));
        Day18.Solve(_inputs[17], new(_part1Buffer, _part2Buffer));
        Day19.Solve(_inputs[18], new(_part1Buffer, _part2Buffer));
        Day20.Solve(_inputs[19], new(_part1Buffer, _part2Buffer));
        Day21.Solve(_inputs[20], new(_part1Buffer, _part2Buffer));
        Day22.Solve(_inputs[21], new(_part1Buffer, _part2Buffer));
        Day23.Solve(_inputs[22], new(_part1Buffer, _part2Buffer));
        Day24.Solve(_inputs[23], new(_part1Buffer, _part2Buffer));
        Day25.Solve(_inputs[24], new(_part1Buffer, _part2Buffer));
    }
}
