using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2017.Solvers
{
    public class Day1 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            int len = input.Length;
            int mid = len / 2;
            int part1 = 0;
            int part2 = 0;
            for (int i = 0; i < len; i++)
            {
                char cur = input[i];
                if (cur == input[(i + 1) % len])
                {
                    part1 += cur - '0';
                }

                if (cur == input[(i + mid) % len])
                {
                    part2 += cur - '0';
                }
            }

            return new Solution(part1.ToString(), part2.ToString());
        }
    }
}
