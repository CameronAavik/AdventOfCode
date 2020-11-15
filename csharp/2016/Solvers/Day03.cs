using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers
{
    public class Day03 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            int part1Total = 0;
            int part2Total = 0;

            int part2SideNum = 0;
            Span<int> prevSides = stackalloc int[6];
            foreach (ReadOnlySpan<char> line in input.Split('\n'))
            {
                ParseLine(line, out int side1, out int side2, out int side3);

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

            return new Solution(part1Total, part2Total);
        }

        private static void ParseLine(ReadOnlySpan<char> line, out int side1, out int side2, out int side3)
        {
            ReadOnlySpan<char> trimmed = line.TrimStart(' ');
            int spaceIndex = trimmed.IndexOf(' ');
            side1 = int.Parse(trimmed.Slice(0, spaceIndex));

            trimmed = trimmed.Slice(spaceIndex).TrimStart(' ');
            spaceIndex = trimmed.IndexOf(' ');
            side2 = int.Parse(trimmed.Slice(0, spaceIndex));

            spaceIndex = trimmed.LastIndexOf(' ');
            side3 = int.Parse(trimmed.Slice(spaceIndex + 1));
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
                int t = i;
                i = j;
                j = t;
            }
        }
    }
}
