using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;
public class Day03 : ISolver
{
    public Solution Solve(ReadOnlySpan<char> input)

    {
        int w = input.IndexOf('\n');
        int h = (input.Length + 1) / (w + 1);

        long slope_1_1 = CountTrees(input, w, h, 1, 1);
        long slope_3_1 = CountTrees(input, w, h, 3, 1);
        long slope_5_1 = CountTrees(input, w, h, 5, 1);
        long slope_7_1 = CountTrees(input, w, h, 7, 1);
        long slope_1_2 = CountTrees(input, w, h, 1, 2);

        long part1 = slope_3_1;
        long part2 = slope_1_1 * slope_3_1 * slope_5_1 * slope_7_1 * slope_1_2;

        return new Solution(part1.ToString(), part2.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static long CountTrees(ReadOnlySpan<char> input, int w, int h, int dx, int dy)
    {
        int stride = w + 1;
        int x = 0;
        long total = 0;
        for (int y = 0; y < h; y += dy)
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
