using System;
using System.Collections.Generic;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
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
            int i = 0;
            while (!reader.Done)
            {
                if (reader.Peek() == 'x')
                {
                    reader.SkipLength("x,".Length);
                }
                else
                {
                    var busId = reader.ReadPosIntUntil(',');

                    // Part 1
                    int firstTimeAfterEarliest = (earliestTime / busId) * busId;
                    if (earliestTime % busId != 0)
                    {
                        firstTimeAfterEarliest += busId;
                    }

                    if (firstTimeAfterEarliest < part1Time)
                    {
                        part1Id = busId;
                        part1Time = firstTimeAfterEarliest;
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

                i++;
            }

            int part1 = part1Id * (part1Time - earliestTime);

            BigInteger part2 = 0;
            foreach ((var n, var a) in buses)
            {
                var p = product / n;
                part2 += a * BigInteger.ModPow(p, n - 2, n) * p;
            }

            part2 %= product;

            return new Solution(part1.ToString(), part2.ToString());
        }
    }
}
