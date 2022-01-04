using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2017.Solvers;

public class Day01 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int len = input.TrimEnd((byte)'\n').Length;
        int mid = len / 2;
        int part1 = 0;
        int part2 = 0;
        for (int i = 0; i < len; i++)
        {
            byte cur = input[i];
            if (cur == input[(i + 1) % len])
            {
                part1 += cur - '0';
            }

            if (cur == input[(i + mid) % len])
            {
                part2 += cur - '0';
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
