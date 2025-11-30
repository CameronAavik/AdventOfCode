using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        foreach (var presentRange in input.SplitLines())
        {
            ParseDimensionString(input[presentRange], out var side0, out var side1, out var side2);

            // ensure side2 is the largest side, side0 and side1 don't have to be sorted.
            SwapIfGreater(ref side1, ref side2);
            SwapIfGreater(ref side0, ref side2);

            var smallestFaceArea = side0 * side1;
            var smallestFacePerimeter = 2 * (side0 + side1);
            part1 += 3 * smallestFaceArea + smallestFacePerimeter * side2;
            part2 += smallestFacePerimeter + smallestFaceArea * side2;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void ParseDimensionString(ReadOnlySpan<byte> present, out int side0, out int side1, out int side2)
    {
        var i = 0;
        side0 = ParseIntUntilX(present, ref i);
        side1 = ParseIntUntilX(present, ref i);
        side2 = ParseIntUntilEnd(present, ref i);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ParseIntUntilX(ReadOnlySpan<byte> present, ref int i)
    {
        byte c;
        var val = present[i++] - '0';
        while ((c = present[i++]) != 'x')
        {
            val = 10 * val + (c - '0');
        }

        return val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ParseIntUntilEnd(ReadOnlySpan<byte> present, ref int i)
    {
        var val = present[i++] - '0';
        while (i < present.Length)
        {
            val = 10 * val + (present[i++] - '0');
        }

        return val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SwapIfGreater(ref int i, ref int j)
    {
        if (i > j)
        {
            (j, i) = (i, j);
        }
    }
}
