using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day01 : ISolver
{
    public Solution Solve(ReadOnlySpan<char> input)
    {
        var spanReader = new SpanReader(input);

        int part1 = 0;
        int part2 = 0;
        int a = -1, b = -1, c = -1;

        while (!spanReader.Done)
        {
            int m = spanReader.ReadPosIntUntil('\n');

            if (c >= 0 && m > c)
                part1++;

            if (a >= 0 && m > a)
                part2++;

            (a, b, c) = (b, c, m);
        }

        return new Solution(part1, part2);
    }
}
