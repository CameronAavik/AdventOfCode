using System;
using System.Globalization;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        long part1 = 0;
        long part2 = 0;

        foreach (var range in input.TrimEnd((byte)'\n').Split((byte)','))
        {
            var rangeStr = input[range];
            var dashIndex = rangeStr.IndexOf((byte)'-');
            var start = long.Parse(rangeStr[..dashIndex], CultureInfo.InvariantCulture);
            var end = long.Parse(rangeStr[(dashIndex + 1)..], CultureInfo.InvariantCulture);

            var digits = 2;
            var digitsStart = 10L;
            var digitsEnd = 99L;
            while (end >= digitsStart)
            {
                if (start <= digitsEnd)
                    AddInvalidIds(digits, Math.Max(start, digitsStart), Math.Min(end, digitsEnd), ref part1, ref part2);

                digits++;
                digitsStart *= 10;
                digitsEnd = (digitsEnd + 1) * 10 - 1;
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part1 + part2);
    }

    private static void AddInvalidIds(int digits, long start, long end, ref long part1, ref long part2)
    {
        switch (digits)
        {
            case 2:
                part1 += SumInRangeWithPattern(start, end, 11);
                break;
            case 3:
                part2 += SumInRangeWithPattern(start, end, 111);
                break;
            case 4:
                part1 += SumInRangeWithPattern(start, end, 0101);
                break;
            case 5:
                part2 += SumInRangeWithPattern(start, end, 11111);
                break;
            case 6:
                part1 += SumInRangeWithPattern(start, end, 001001);
                part2 += SumInRangeWithPattern(start, end, 010101);
                part2 -= SumInRangeWithPattern(start, end, 111111);
                break;
            case 7:
                part2 += SumInRangeWithPattern(start, end, 1111111);
                break;
            case 8:
                part1 += SumInRangeWithPattern(start, end, 000010001);
                break;
            case 9:
                part2 += SumInRangeWithPattern(start, end, 001001001);
                break;
            case 10:
                part1 += SumInRangeWithPattern(start, end, 0000100001);
                part2 += SumInRangeWithPattern(start, end, 0101010101);
                part2 -= SumInRangeWithPattern(start, end, 1111111111);
                break;
        }
    }

    private static long SumInRangeWithPattern(long start, long end, long pattern)
    {
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
