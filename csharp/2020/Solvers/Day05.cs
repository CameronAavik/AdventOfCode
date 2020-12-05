using System;
using System.IO;
using System.Numerics;
using System.Security;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers
{
    public class Day05 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            uint[] seen = new uint[32];
            seen[0] |= 0xFFu; // fill in first row

            for (int i = 0; i < input.Length; i += 11)
            {
                int seatId = 0;
                for (int j = i; j < i + 10; j++)
                {
                    seatId <<= 1;
                    if (input[j] is 'B' or 'R')
                    {
                        seatId |= 1;
                    }
                }

                seen[seatId / 32] |= 1u << (seatId % 32);
            }

            int part1 = 0;
            int part2 = 0;
            for (int i = 0; i < seen.Length; i++)
            {
                var elem = seen[i];
                if (elem != 0xFFFFFFFFu)
                {
                    if (part2 == 0)
                    {
                        part2 = 32 * i + (31 - BitOperations.LeadingZeroCount(~elem));
                    }
                    else
                    {
                        part1 = 32 * i + (31 - BitOperations.LeadingZeroCount(elem));
                        break;
                    }
                }
            }

            return new Solution(part1, part2);
        }
    }
}
