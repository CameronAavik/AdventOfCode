using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day05 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            uint[] seen = new uint[32];
            seen[0] |= 0xFFu; // fill in first row

            int part1 = 0;
            for (int i = 0; i < input.Length; i += 11)
            {
                // unrolling the loop to save precious milliseconds
                int seatId = 0;
                if (input[i + 0] == 'B') seatId |= 1 << 9;
                if (input[i + 1] == 'B') seatId |= 1 << 8;
                if (input[i + 2] == 'B') seatId |= 1 << 7;
                if (input[i + 3] == 'B') seatId |= 1 << 6;
                if (input[i + 4] == 'B') seatId |= 1 << 5;
                if (input[i + 5] == 'B') seatId |= 1 << 4;
                if (input[i + 6] == 'B') seatId |= 1 << 3;
                if (input[i + 7] == 'R') seatId |= 1 << 2;
                if (input[i + 8] == 'R') seatId |= 1 << 1;
                if (input[i + 9] == 'R') seatId |= 1 << 0;

                part1 = Math.Max(part1, seatId);
                seen[seatId / 32] |= 1u << (seatId % 32);
            }

            for (int i = 0; i < seen.Length; i++)
            {
                var elem = seen[i];
                if (elem != 0xFFFFFFFFu)
                {
                    int part2 = 32 * i + (31 - BitOperations.LeadingZeroCount(~elem));
                    return new Solution(part1, part2);
                }
            }

            return new Solution(part1, -1);
        }
    }
}
