using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2016.Solvers
{
    public class Day01 : ISolver
    {
        private const uint Up = 1;
        private const uint Down = unchecked((uint)-Up);
        private const uint Right = 1 << 16;
        private const uint Left = unchecked((uint)-Right);

        public Solution Solve(ReadOnlySpan<char> input)
        {
            // assume the origin is at 1 << 15, 1 << 15
            const ushort xOrigin = 1 << 15;
            const ushort yOrigin = 1 << 15;

            // pack the x and y ushorts into a uint
            uint pos = unchecked((uint)xOrigin << 16 | yOrigin);
            uint dir = Up;

            var seenLocations = new HashSet<uint> { pos };
            int distanceToFirstRepeatedLocation = -1;
            foreach (ReadOnlySpan<char> instruction in input.Split(", "))
            {
                dir = MakeTurn(dir, instruction[0] == 'L');

                uint distance = uint.Parse(instruction[1..]);
                if (distanceToFirstRepeatedLocation == -1)
                {
                    for (uint i = 0; i < distance; i++)
                    {
                        pos = unchecked(pos + dir);
                        if (!seenLocations.Add(pos))
                        {
                            distanceToFirstRepeatedLocation = ManhattanDistance(pos);
                        }
                    }
                }
                else
                {
                    pos = unchecked(pos + dir * distance);
                }
            }

            int distanceToDestination = ManhattanDistance(pos);

            return new Solution(
                part1: distanceToDestination,
                part2: distanceToFirstRepeatedLocation);

            static uint MakeTurn(uint dir, bool isLeft) => dir switch
            {
                Left => isLeft ? Down : Up,
                Down => isLeft ? Right : Left,
                Right => isLeft ? Up : Down,
                Up => isLeft ? Left : Right,
                _ => default,
            };

            static int ManhattanDistance(uint pos)
            {
                int xAbs = Math.Abs((int)(pos >> 16) - xOrigin);
                int yAbs = Math.Abs((int)(pos & 0xFFFF) - yOrigin);
                return xAbs + yAbs;
            }
        }
    }
}
