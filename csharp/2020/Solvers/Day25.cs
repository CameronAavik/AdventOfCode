using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day25 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            var reader = new SpanReader(input);
            int key1 = reader.ReadPosIntUntil('\n');
            int key2 = reader.ReadPosIntUntil('\n');

            int n = 1;
            int loopSize = 0;
            while (n != key1)
            {
                n = (7 * n) % 20201227;
                loopSize++;
            }

            BigInteger part1 = BigInteger.ModPow(key2, loopSize, 20201227);

            return new Solution(part1.ToString(), string.Empty);
        }
    }
}
