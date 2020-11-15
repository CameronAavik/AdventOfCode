using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day25 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            ReadOnlySpan<char> trimmed = input[80..]; // Trims redundant first 80 characters and leaves behind "3010, column 3019."
            int row = int.Parse(trimmed[..trimmed.IndexOf(',')]);
            int column = int.Parse(trimmed[(trimmed.LastIndexOf(' ') + 1)..^1]);

            int n = row + column - 1;
            int diagEnd = n * (n + 1) / 2;
            int repetitions = diagEnd - row;

            BigInteger part1 = (BigInteger.ModPow(252533, repetitions, 33554393) * 20151125) % 33554393;
            return new Solution(part1.ToString(), string.Empty);
        }
    }
}
