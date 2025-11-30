using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day03 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)

    {
        var w = input.IndexOf((byte)'\n');
        var h = (input.Length + 1) / (w + 1);

        var slope_1_1 = CountTrees(input, w, h, 1, 1);
        var slope_3_1 = CountTrees(input, w, h, 3, 1);
        var slope_5_1 = CountTrees(input, w, h, 5, 1);
        var slope_7_1 = CountTrees(input, w, h, 7, 1);
        var slope_1_2 = CountTrees(input, w, h, 1, 2);

        var part1 = slope_3_1;
        var part2 = slope_1_1 * slope_3_1 * slope_5_1 * slope_7_1 * slope_1_2;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static long CountTrees(ReadOnlySpan<byte> input, int w, int h, int dx, int dy)
    {
        var stride = w + 1;
        var x = 0;
        long total = 0;
        for (var y = 0; y < h; y += dy)
        {
            if (input[y * stride + x] == '#')
            {
                total += 1;
            }

            x += dx;
            if (x >= w)
                x -= w;
        }

        return total;
    }
}
