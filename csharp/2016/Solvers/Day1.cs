using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2016.Solvers
{
    public class Day1 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            // -x = left, +x = right
            // -y = down, +y = up
            var pos = new Vec2 { X = 0, Y = 0 };
            var dir = new Vec2 { X = 0, Y = 1 };

            var seenLocations = new HashSet<Vec2> { pos };
            int distanceToFirstRepeatedLocation = -1;

            foreach (ReadOnlySpan<char> instruction in input.Split(", "))
            {
                dir = instruction[0] == 'L' 
                    ? new Vec2 { X = -dir.Y, Y = dir.X }
                    : new Vec2 { X = dir.Y, Y = -dir.X };

                int distance = Int32.Parse(instruction[1..]);
                if (distanceToFirstRepeatedLocation == -1)
                {
                    for (int i = 0; i < distance; i++)
                    {
                        pos += dir;
                        if (!seenLocations.Add(pos))
                        {
                            distanceToFirstRepeatedLocation = ManhattanDistance(pos);
                        }
                    }
                }
                else
                {
                    pos += dir * distance;
                }
            }

            int distanceToDestination = ManhattanDistance(pos);

            return new Solution(
                part1: distanceToDestination.ToString(),
                part2: distanceToFirstRepeatedLocation.ToString());
        }

        private static int ManhattanDistance(Vec2 vec) => Math.Abs(vec.X) + Math.Abs(vec.Y);
    }
}
