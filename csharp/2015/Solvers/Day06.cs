using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day06 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            const int part1Mask = 1 << 16;
            int[,] grid = new int[1000, 1000];

            foreach (var instruction in input.Split('\n'))
            {
                int x, y;
                int x1, y1, x2, y2;
                switch (instruction[6])
                {
                    case ' ':
                        // toggle
                        ParseArea(instruction[7..], out x1, out y1, out x2, out y2);

                        for (x = x1; x <= x2; x++)
                        {
                            for (y = y1; y <= y2; y++)
                            {
                                grid[x, y] = (grid[x, y] ^ part1Mask) + 2;
                            }
                        }

                        break;
                    case 'f':
                        // turn off
                        ParseArea(instruction[9..], out x1, out y1, out x2, out y2);

                        for (x = x1; x <= x2; x++)
                        {
                            for (y = y1; y <= y2; y++)
                            {
                                grid[x, y] = Math.Max((grid[x, y] & 0xFFFF) - 1, 0);
                            }
                        }

                        break;
                    case 'n':
                        // turn on
                        ParseArea(instruction[8..], out x1, out y1, out x2, out y2);

                        for (x = x1; x <= x2; x++)
                        {
                            for (y = y1; y <= y2; y++)
                            {
                                grid[x, y] = (grid[x, y] | part1Mask) + 1;
                            }
                        }

                        break;
                }
            }

            int part1 = 0;
            int part2 = 0;
            foreach (int gridValue in grid)
            {
                part1 += gridValue >> 16;
                part2 += gridValue & 0xFFFF;
            }

            return new Solution(part1, part2);
        }

        private static void ParseArea(ReadOnlySpan<char> regionStr, out int x1, out int y1, out int x2, out int y2)
        {
            char c;
            int i = 0;

            x1 = 0;
            while ((c = regionStr[i++]) != ',')
            {
                x1 = x1 * 10 + (c - '0');
            }

            y1 = 0;
            while ((c = regionStr[i++]) != ' ')
            {
                y1 = y1 * 10 + (c - '0');
            }

            i += 8; // length of "through "

            x2 = 0;
            while ((c = regionStr[i++]) != ',')
            {
                x2 = x2 * 10 + (c - '0');
            }

            y2 = 0;
            while (i < regionStr.Length)
            {
                y2 = y2 * 10 + (regionStr[i++] - '0');
            }
        }
    }
}
