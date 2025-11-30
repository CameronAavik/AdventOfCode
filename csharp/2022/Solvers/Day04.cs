using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day04 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var inputCursor = 0;

        var part1 = 0;
        var part2 = 0;

        while (inputCursor < input.Length)
        {
            var start1 = ReadIntegerUntil(input, '-', ref inputCursor);
            var end1 = ReadIntegerUntil(input, ',', ref inputCursor);
            var start2 = ReadIntegerUntil(input, '-', ref inputCursor);
            var end2 = ReadIntegerUntil(input, '\n', ref inputCursor);

            if (start2 <= end1 && start1 <= end2)
            {
                part2++;

                if ((start1 <= start2 && end2 <= end1) || (start2 <= start1 && end1 <= end2))
                {
                    part1++;
                }
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    public static int ReadIntegerUntil(ReadOnlySpan<byte> span, char c, ref int i)
    {
        // Assume that the first character is always a digit
        var ret = span[i++] - '0';

        byte cur;
        while ((cur = span[i++]) != c)
            ret = ret * 10 + (cur - '0');

        return ret;
    }
}
