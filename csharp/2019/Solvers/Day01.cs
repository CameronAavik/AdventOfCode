using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2019.Solvers;

public class Day01 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var reader = new SpanReader(input);

        int part1 = 0;
        int part2 = 0;
        while (!reader.Done)
        {
            int mass = reader.ReadPosIntUntil('\n');
            int fuel = mass / 3 - 2;
            part1 += fuel;
            while (fuel > 0)
            {
                part2 += fuel;
                fuel = fuel / 3 - 2;
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
