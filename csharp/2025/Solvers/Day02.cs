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
        var startTop = start / 10;
        if (startTop * 11 < start)
            startTop++;

        var endTop = end / 10;
        if (endTop * 11 > end)
            endTop--;

        var total = SumRangeInclusive(startTop, endTop) * 11;
        part1 += total;
        part2 += total;
    }

    private static void HandleThreeDigits(long start, long end, ref long _, ref long part2)
    {
        var startTop = start / 100;
        if (startTop * 111 < start)
            startTop++;

        var endTop = end / 100;
        if (endTop * 111 > end)
            endTop--;

        part2 += SumRangeInclusive(startTop, endTop) * 111;
    }

    private static void HandleFourDigits(long start, long end, ref long part1, ref long part2)
    {
        var startTop = start / 100;
        if (startTop * 101 < start)
            startTop++;

        var endTop = end / 100;
        if (endTop * 101 > end)
            endTop--;

        var total = SumRangeInclusive(startTop, endTop) * 101;
        part1 += total;
        part2 += total;
    }

    private static void HandleFiveDigits(long start, long end, ref long _, ref long part2)
    {
        var startTop = start / 10000;
        if (startTop * 11111 < start)
            startTop++;

        var endTop = end / 10000;
        if (endTop * 11111 > end)
            endTop--;

        part2 += SumRangeInclusive(startTop, endTop) * 11111;
    }

    private static void HandleSixDigits(long start, long end, ref long part1, ref long part2)
    {
        // XYZXYZ
        var startTop = start / 1000;
        if (startTop * 1001 < start)
            startTop++;

        var endTop = end / 1000;
        if (endTop * 1001 > end)
            endTop--;

        var total = SumRangeInclusive(startTop, endTop) * 1001;
        part1 += total;
        part2 += total;

        // XYXYXY
        startTop = start / 10000;
        if (startTop * 10101 < start)
            startTop++;

        endTop = end / 10000;
        if (endTop * 10101 > end)
            endTop--;

        part2 += SumRangeInclusive(startTop, endTop) * 10101;

        // XXXXXX (need to subtract)
        startTop = start / 100000;
        if (startTop * 111111 < start)
            startTop++;

        endTop = end / 100000;
        if (endTop * 111111 > end)
            endTop--;

        part2 -= SumRangeInclusive(startTop, endTop) * 111111;
    }

    private static void HandleSevenDigits(long start, long end, ref long _, ref long part2)
    {
        var startTop = start / 1000000;
        if (startTop * 1111111 < start)
            startTop++;

        var endTop = end / 1000000;
        if (endTop * 1111111 > end)
            endTop--;

        part2 += SumRangeInclusive(startTop, endTop) * 1111111;
    }

    private static void HandleEightDigits(long start, long end, ref long part1, ref long part2)
    {
        // for both parts, all numbers of form WXYZWXYZ
        var startTop = start / 10000;
        if (startTop * 10001 < start)
            startTop++;

        var endTop = end / 10000;
        if (endTop * 10001 > end)
            endTop--;

        var total = SumRangeInclusive(startTop, endTop) * 10001;
        part1 += total;
        part2 += total;
    }

    private static void HandleNineDigits(long start, long end, ref long _, ref long part2)
    {
        var startTop = start / 1000000;
        if (startTop * 1001001 < start)
            startTop++;

        var endTop = end / 1000000;
        if (endTop * 1001001 > end)
            endTop--;

        part2 += SumRangeInclusive(startTop, endTop) * 1001001;
    }

    private static void HandleTenDigits(long start, long end, ref long part1, ref long part2)
    {
        // VWXYZVWXYZ
        var startTop = start / 100000;
        if (startTop * 100001 < start)
            startTop++;

        var endTop = end / 100000;
        if (endTop * 100001 > end)
            endTop--;

        var total = SumRangeInclusive(startTop, endTop) * 100001;
        part1 += total;
        part2 += total;

        // XYXYXYXYXY
        startTop = start / 100000000L;
        if (startTop * 101010101 < start)
            startTop++;

        endTop = end / 100000000L;
        if (endTop * 101010101 > end)
            endTop--;

        part2 += SumRangeInclusive(startTop, endTop) * 101010101;

        // XXXXXXXXXX (need to subtract)
        startTop = start / 1000000000L;
        if (startTop * 1111111111 < start)
            startTop++;

        endTop = end / 1000000000L;
        if (endTop * 1111111111 > end)
            endTop--;

        part2 -= SumRangeInclusive(startTop, endTop) * 1111111111;
    }

    private static long SumRangeInclusive(long start, long end)
    {
        return (end * (end + 1) / 2) - ((start - 1) * start / 2);
    }
}
