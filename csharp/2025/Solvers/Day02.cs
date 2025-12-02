using System;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
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
        solution.SubmitPart2(part2);
    }

    private static void AddInvalidIds(int digits, long start, long end, ref long part1, ref long part2)
    {
        switch (digits)
        {
            case 0:
            case 1:
                return;
            case 2:
                HandleTwoDigits(start, end, ref part1, ref part2);
                break;
            case 3:
                HandleThreeDigits(start, end, ref part1, ref part2);
                break;
            case 4:
                HandleFourDigits(start, end, ref part1, ref part2);
                break;
            case 5:
                HandleFiveDigits(start, end, ref part1, ref part2);
                break;
            case 6:
                HandleSixDigits(start, end, ref part1, ref part2);
                break;
            case 7:
                HandleSevenDigits(start, end, ref part1, ref part2);
                break;
            case 8:
                HandleEightDigits(start, end, ref part1, ref part2);
                break;
            case 9:
                HandleNineDigits(start, end, ref part1, ref part2);
                break;
            case 10:
                HandleTenDigits(start, end, ref part1, ref part2);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private static void HandleTwoDigits(long start, long end, ref long part1, ref long part2)
    {
        var startTop = RoundUpNearestMultiple(start, 11) / 10;
        var endTop = RoundDownNearestMultiple(end, 11) / 10;
        var total = SumRangeInclusive(startTop, endTop) * 11;
        part1 += total;
        part2 += total;
    }

    private static void HandleThreeDigits(long start, long end, ref long _, ref long part2)
    {
        var startTop = RoundUpNearestMultiple(start, 111) / 100;
        var endTop = RoundDownNearestMultiple(end, 111) / 100;
        part2 += SumRangeInclusive(startTop, endTop) * 111;
    }

    private static void HandleFourDigits(long start, long end, ref long part1, ref long part2)
    {
        var startTop = RoundUpNearestMultiple(start, 101) / 100;
        var endTop = RoundDownNearestMultiple(end, 101) / 100;
        var total = SumRangeInclusive(startTop, endTop) * 101;
        part1 += total;
        part2 += total;
    }

    private static void HandleFiveDigits(long start, long end, ref long _, ref long part2)
    {
        var startTop = RoundUpNearestMultiple(start, 11111) / 10000;
        var endTop = RoundDownNearestMultiple(end, 11111) / 10000;
        part2 += SumRangeInclusive(startTop, endTop) * 11111;
    }

    private static void HandleSixDigits(long start, long end, ref long part1, ref long part2)
    {
        // XYZXYZ
        var startTop = RoundUpNearestMultiple(start, 1001) / 1000;
        var endTop = RoundDownNearestMultiple(end, 1001) / 1000;
        var total = SumRangeInclusive(startTop, endTop) * 1001;
        part1 += total;
        part2 += total;

        // XYXYXY
        startTop = RoundUpNearestMultiple(start, 10101) / 10000;
        endTop = RoundDownNearestMultiple(end, 10101) / 10000;
        part2 += SumRangeInclusive(startTop, endTop) * 10101;

        // XXXXXX (need to subtract)
        startTop = RoundUpNearestMultiple(start, 111111) / 100000;
        endTop = RoundDownNearestMultiple(end, 111111) / 100000;
        part2 -= SumRangeInclusive(startTop, endTop) * 111111;
    }

    private static void HandleSevenDigits(long start, long end, ref long _, ref long part2)
    {
        var startTop = RoundUpNearestMultiple(start, 1111111) / 1000000;
        var endTop = RoundDownNearestMultiple(end, 1111111) / 1000000;
        part2 += SumRangeInclusive(startTop, endTop) * 1111111;
    }

    private static void HandleEightDigits(long start, long end, ref long part1, ref long part2)
    {
        // for both parts, all numbers of form WXYZWXYZ
        var startTop = RoundUpNearestMultiple(start, 10001) / 10000;
        var endTop = RoundDownNearestMultiple(end, 10001) / 10000;
        var total = SumRangeInclusive(startTop, endTop) * 10001;
        part1 += total;
        part2 += total;
    }

    private static void HandleNineDigits(long start, long end, ref long _, ref long part2)
    {
        var startTop = RoundUpNearestMultiple(start, 1001001) / 1000000;
        var endTop = RoundDownNearestMultiple(end, 1001001) / 1000000;
        part2 += SumRangeInclusive(startTop, endTop) * 1001001;
    }

    private static void HandleTenDigits(long start, long end, ref long part1, ref long part2)
    {
        // VWXYZVWXYZ
        var startTop = RoundUpNearestMultiple(start, 100001) / 100000;
        var endTop = RoundDownNearestMultiple(end, 100001) / 100000;
        var total = SumRangeInclusive(startTop, endTop) * 100001;
        part1 += total;
        part2 += total;

        // XYXYXYXYXY
        startTop = RoundUpNearestMultiple(start, 101010101) / 100000000;
        endTop = RoundDownNearestMultiple(end, 101010101) / 100000000;
        part2 += SumRangeInclusive(startTop, endTop) * 101010101;

        // XXXXXXXXXX (need to subtract)
        startTop = RoundUpNearestMultiple(start, 1111111111) / 1000000000L;
        endTop = RoundDownNearestMultiple(end, 1111111111) / 1000000000L;
        part2 -= SumRangeInclusive(startTop, endTop) * 1111111111;
    }

    private static long RoundUpNearestMultiple(long value, long multiple)
    {
        var remainder = value % multiple;
        if (remainder == 0)
            return value;
        return value + (multiple - remainder);
    }

    private static long RoundDownNearestMultiple(long value, long multiple)
    {
        return value - (value % multiple);
    }

    private static long SumRangeInclusive(long start, long end)
    {
        return (end * (end + 1) / 2) - ((start - 1) * start / 2);
    }
}
