using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day25 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            var trimmed = input[80..]; // Trims redundant first 80 characters and leaves behind "3010, column 3019."
            int row = Int32.Parse(trimmed[..(trimmed.IndexOf(','))]);
            int column = Int32.Parse(trimmed[(trimmed.LastIndexOf(' ') + 1)..^1]);

            int n = row + column - 1;
            int diagEnd = n * (n + 1) / 2;
            int repetitions = diagEnd - row;

            var part1 = (BigInteger.ModPow(252533, repetitions, 33554393) * 20151125) % 33554393;
            return new Solution(part1.ToString(), String.Empty);
        }
    }
}
