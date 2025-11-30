using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day03 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1Total = 0;
        var part2Total = 0;

        var part2SideNum = 0;
        Span<int> prevSides = stackalloc int[6];
        foreach (var lineRange in input.SplitLines())
        {
            ParseLine(input[lineRange], out var side1, out var side2, out var side3);

            if (IsValidTriangle(side1, side2, side3))
            {
                part1Total++;
            }

            if (part2SideNum == 2)
            {
                if (IsValidTriangle(prevSides[0], prevSides[3], side1))
                {
                    part2Total++;
                }

                if (IsValidTriangle(prevSides[1], prevSides[4], side2))
                {
                    part2Total++;
                }

                if (IsValidTriangle(prevSides[2], prevSides[5], side3))
                {
                    part2Total++;
                }

                part2SideNum = 0;
            }
            else
            {
                prevSides[3 * part2SideNum] = side1;
                prevSides[3 * part2SideNum + 1] = side2;
                prevSides[3 * part2SideNum + 2] = side3;
                part2SideNum++;
            }
        }

        solution.SubmitPart1(part1Total);
        solution.SubmitPart2(part2Total);
    }

    private static void ParseLine(ReadOnlySpan<byte> line, out int side1, out int side2, out int side3)
    {
        var reader = new SpanReader(line);
        reader.SkipWhile(' ');
        side1 = reader.ReadPosIntUntil(' ');
        reader.SkipWhile(' ');
        side2 = reader.ReadPosIntUntil(' ');
        reader.SkipWhile(' ');
        side3 = reader.ReadPosIntUntilEnd();
    }

    private static bool IsValidTriangle(int side1, int side2, int side3)
    {
        // ensure that side3 is the largest side
        SwapIfGreater(ref side1, ref side3);
        SwapIfGreater(ref side2, ref side3);
        return side1 + side2 > side3;
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
