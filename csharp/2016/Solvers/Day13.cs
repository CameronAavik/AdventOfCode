using System;
using System.Collections.Generic;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day13 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var favouriteNumber = new SpanReader(input).ReadPosIntUntil('\n');

        var seen = new HashSet<(int, int)>();

        var steps = 0;
        var frontier = new HashSet<(int, int)> { (1, 1) };

        int? part1 = null;
        int? part2 = null;
        while (part1 == null || part2 == null)
        {
            var newFrontier = new HashSet<(int, int)>();
            foreach ((var x, var y) in frontier)
            {
                if (x == 31 && y == 39)
                    part1 = steps;

                if (IsWall(x, y) || seen.Contains((x, y)))
                    continue;

                seen.Add((x, y));

                newFrontier.Add((x - 1, y));
                newFrontier.Add((x + 1, y));
                newFrontier.Add((x, y - 1));
                newFrontier.Add((x, y + 1));
            }

            if (steps == 50)
                part2 = seen.Count;

            frontier = newFrontier;
            steps++;
        }

        solution.SubmitPart1(part1.Value);
        solution.SubmitPart2(part2.Value);

        bool IsWall(int x, int y)
        {
            if (x < 0 || y < 0)
                return true;

            var sum = x * x + 3 * x + 2 * x * y + y + y * y + favouriteNumber;
            return BitOperations.PopCount((uint)sum) % 2 == 1;
        }
    }
}
