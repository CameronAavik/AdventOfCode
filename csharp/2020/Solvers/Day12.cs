using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day12 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            // Part 1
            int x1 = 0, y1 = 0;
            int dx1 = 1, dy1 = 0;

            // Part 2
            int x2 = 0, y2 = 0;
            int dx2 = 10, dy2 = 1;

            var reader = new SpanReader(input);
            while (!reader.Done)
            {
                var dir = reader.Peek();
                reader.SkipLength(1);
                var amount = reader.ReadPosIntUntil('\n');

                switch (dir)
                {
                    case 'N':
                        y1 += amount;
                        dy2 += amount;
                        break;
                    case 'S':
                        y1 -= amount;
                        dy2 -= amount;
                        break;
                    case 'E':
                        x1 += amount;
                        dx2 += amount;
                        break;
                    case 'W':
                        x1 -= amount;
                        dx2 -= amount;
                        break;
                    case 'L':
                        for (int i = amount; i > 0; i -= 90)
                        {
                            RotateLeft(ref dx1, ref dy1);
                            RotateLeft(ref dx2, ref dy2);
                        }
                        break;
                    case 'R':
                        for (int i = amount; i > 0; i -= 90)
                        {
                            RotateRight(ref dx1, ref dy1);
                            RotateRight(ref dx2, ref dy2);
                        }
                        break;
                    case 'F':
                        x1 += amount * dx1;
                        y1 += amount * dy1;

                        x2 += amount * dx2;
                        y2 += amount * dy2;
                        break;
                }
            }

            int part1 = Math.Abs(x1) + Math.Abs(y1);
            int part2 = Math.Abs(x2) + Math.Abs(y2);
            return new Solution(part1, part2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RotateLeft(ref int dx, ref int dy)
        {
            int temp = dx;
            dx = -dy;
            dy = temp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RotateRight(ref int dx, ref int dy)
        {
            int temp = dx;
            dx = dy;
            dy = -temp;
        }
    }
}
