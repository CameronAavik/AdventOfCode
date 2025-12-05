using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2025.Solvers;

public class Day05 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        Span<long> rangeEvents = new long[512];
        var ptr = 0;

        var i = 0;
        while (input[i] != '\n')
        {
            var prevI = i;
            var start = ParseUntil(input, (byte)'-', ref i);

            // We know that the end value will have at least the same number of digits as the start value
            var startLen = i - prevI - 1;
            var end = ParseWithMinLengthUntil(input, startLen, (byte)'\n', ref i);

            // Store events in a way that we use the least significant bit to indicate start/end
            // When sorting, this ensures that start events come before end events, so if one range's start and
            // another range's end are equal, they will get merged together rather than treated as separate ranges.
            rangeEvents[ptr++] = start * 2;
            rangeEvents[ptr++] = end * 2 + 1;
        }

        rangeEvents = rangeEvents[..ptr];
        rangeEvents.Sort();

        ptr = 0;

        long part2 = 0;

        // Merge overlapping ranges by keeping track of the current depth from start and end events
        // Whenever the depth is 0, it means there are no currently overlapping ranges
        var depth = 0;
        for (var j = 0; j < rangeEvents.Length; j++)
        {
            var ev = rangeEvents[j];
            if (depth == 0)
            {
                var v = ev / 2;
                rangeEvents[ptr++] = v;

                // Since the width of a range is (end - start + 1), we subtract start now and add end + 1 later
                part2 -= v;
            }

            // Get the bit which indicates if it is a start (0) or end (1) event
            var typeBit = ev & 1;

            // Branchlessly turn start events into +1 and end events into -1
            // -typeBit turns 1 to -1 and 0 to 0, the bitwise or with 1 will keep -1 the same and turn 0 into 1
            depth += (int)(-typeBit | 1);

            if (depth == 0)
            {
                var v = ev / 2;
                rangeEvents[ptr++] = v;
                part2 += v + 1;
            }
        }

        // Every even index is a range start, and every odd index is a range end. All ranges are non-overlapping.
        rangeEvents = rangeEvents[..ptr];

        i++;

        var part1 = 0;
        while (i < input.Length)
        {
            var ingredient = ParseUntil(input, (byte)'\n', ref i);

            // Use binary search to find the index of the range event that is >= ingredient
            var eventIndex = rangeEvents.BinarySearch(ingredient);

            // Span.BinarySearch returns the index if an exact match is found, or the bitwise complement of the index
            // of the next larger element if not found.
            if (eventIndex >= 0)
            {
                // Since the index is positive, and all ranges are inclusive, the ingredient is always fresh.
                part1++;
            }
            else
            {
                // Get the index of the next larger element by inverting the bits
                // If the index of the next event is odd, then it is inside a range
                var greaterIndex = ~eventIndex;
                if (greaterIndex % 2 == 1)
                    part1++;
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static long ParseUntil(ReadOnlySpan<byte> input, byte c, ref int i)
    {
        var result = (long)(input[i++] - '0');
        while (input[i++] is byte d && d != c)
            result = result * 10 + (d - (byte)'0');
        return result;
    }

    private static long ParseWithMinLengthUntil(ReadOnlySpan<byte> input, int minLength, byte c, ref int i)
    {
        var result = (long)(input[i++] - '0');
        for (var j = 1; j < minLength; j++)
            result = result * 10 + (input[i++] - (byte)'0');

        while (input[i++] is byte d && d != c)
            result = result * 10 + (d - (byte)'0');

        return result;
    }
}
