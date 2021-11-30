using System;
using System.Collections.Generic;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day13 : ISolver
{
    public Solution Solve(ReadOnlySpan<char> input)
    {
        var reader = new SpanReader(input.TrimEnd('\n'));
        int earliestTime = reader.ReadPosIntUntil('\n');

        int part1Id = -1;
        int part1Time = int.MaxValue;

        BigInteger product = 1;
        var buses = new List<(int n, int a)>();
        for (int i = 0; !reader.Done; i++)
        {
            if (reader.Peek() == 'x')
            {
                reader.SkipLength("x,".Length);
                continue;
            }

            int busId = reader.ReadPosIntUntil(',');

            // Part 1
            int waitTime = busId - (earliestTime % busId);
            if (waitTime < part1Time)
            {
                part1Id = busId;
                part1Time = waitTime;
            }

            // Part 2
            product *= busId;

            int remainder = (busId - i) % busId;
            if (remainder < 0)
            {
                remainder += busId;
            }

            buses.Add((busId, remainder));
        }

        int part1 = part1Id * part1Time;

        BigInteger part2 = 0;
        foreach ((int n, int a) in buses)
        {
            BigInteger p = product / n;
            part2 += a * BigInteger.ModPow(p, n - 2, n) * p;
        }

        part2 %= product;

        return new Solution(part1.ToString(), part2.ToString());
    }
}
