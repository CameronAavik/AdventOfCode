using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        ulong part1 = 0;
        ulong part2 = 0;

        var i = 0;
        while (i < input.Length)
        {
            var startIndex = i;
            var start = (ulong)input[i++] - '0';
            while (input[i++] is byte c and > (byte)'-')
                start = start * 10 + (ulong)(c - '0');
            var startLen = i - startIndex - 1;

            var endIndex = i;
            var end = (ulong)input[i++] - '0';
            while (input[i++] is byte c and > (byte)',')
                end = end * 10 + (ulong)(c - '0');
            var endLen = i - endIndex - 1;

            for (var digits = startLen; digits <= endLen; digits++)
            {
                switch (digits)
                {
                    case 2:
                        part1 += InvalidIdsWithPattern(start, end, 10, 11);
                        break;
                    case 3:
                        part2 += InvalidIdsWithPattern(start, end, 100, 111);
                        break;
                    case 4:
                        part1 += InvalidIdsWithPattern(start, end, 1000, 0101);
                        break;
                    case 5:
                        part2 += InvalidIdsWithPattern(start, end, 10000, 11111);
                        break;
                    case 6:
                        part1 += InvalidIdsWithPattern(start, end, 100000, 001001);
                        part2 += InvalidIdsWithPattern(start, end, 100000, 010101);
                        part2 -= InvalidIdsWithPattern(start, end, 100000, 111111);
                        break;
                    case 7:
                        part2 += InvalidIdsWithPattern(start, end, 1000000, 1111111);
                        break;
                    case 8:
                        part1 += InvalidIdsWithPattern(start, end, 10000000, 000010001);
                        break;
                    case 9:
                        part2 += InvalidIdsWithPattern(start, end, 100000000, 001001001);
                        break;
                    case 10:
                        part1 += InvalidIdsWithPattern(start, end, 1000000000, 0000100001);
                        part2 += InvalidIdsWithPattern(start, end, 1000000000, 0101010101);
                        part2 -= InvalidIdsWithPattern(start, end, 1000000000, 1111111111);
                        break;
                }
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part1 + part2);
    }

    private static ulong InvalidIdsWithPattern(ulong start, ulong end, ulong min, ulong pattern)
    {
        if (start < min)
            start = min;

        var max = 10 * min;
        if (end > max)
            end = max;

        // Identify the first and last multiplier inside the range
        var first = (start + pattern - 1) / pattern;
        var last = end / pattern;

        var count = last - first + 1;
        if (count <= 0)
            return 0;

        // Sum of arithmetic progression: p * (first + ... + last)
        return pattern * ((first + last) * count / 2);
    }
}
