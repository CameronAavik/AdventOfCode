using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day15 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            // really big array since much faster than a dictionary
            int[] buffer = new int[30000000];

            int i = 1;
            foreach (ReadOnlySpan<char> num in input.Split(','))
            {
                buffer[int.Parse(num)] = i++;
            }

            int cur = 0;
            while (i < 2020)
            {
                int prev_t = buffer[cur];
                int value = prev_t == 0 ? 0 : i - prev_t;
                buffer[cur] = i++;
                cur = value;
            }

            int part1 = cur;

            while (i < 30000000)
            {
                int prev_t = buffer[cur];
                int value = prev_t == 0 ? 0 : i - prev_t;
                buffer[cur] = i++;
                cur = value;
            }

            int part2 = cur;

            return new Solution(part1, part2);
        }
    }
}
