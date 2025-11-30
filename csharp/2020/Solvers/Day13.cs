using System;
using System.Collections.Generic;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day13 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var reader = new SpanReader(input.TrimEnd((byte)'\n'));
        var earliestTime = reader.ReadPosIntUntil('\n');

        var part1Id = -1;
        var part1Time = int.MaxValue;

        BigInteger product = 1;
        var buses = new List<(int n, int a)>();
        for (var i = 0; !reader.Done; i++)
        {
            if (reader.Peek() == 'x')
            {
                reader.SkipLength("x,".Length);
                continue;
            }

            var busId = reader.ReadPosIntUntil(',');

            // Part 1
            var waitTime = busId - (earliestTime % busId);
            if (waitTime < part1Time)
            {
                part1Id = busId;
                part1Time = waitTime;
            }

            // Part 2
            product *= busId;

            var remainder = (busId - i) % busId;
            if (remainder < 0)
            {
                remainder += busId;
            }

            buses.Add((busId, remainder));
        }

        var part1 = part1Id * part1Time;

        BigInteger part2 = 0;
        foreach ((var n, var a) in buses)
        {
            var p = product / n;
            part2 += a * BigInteger.ModPow(p, n - 2, n) * p;
        }

        part2 %= product;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
