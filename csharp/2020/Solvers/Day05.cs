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
            int numSeats = input.Length / 11;

            int maxSeat = 0;
            for (int i = 0; i < input.Length; i += 11)
            {
                // unrolling the loop to save precious microseconds
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

                if (seatId > maxSeat)
                    maxSeat = seatId;

                seen[seatId / 32] |= 1u << (seatId & 31);
            }

            int minSeat = maxSeat - numSeats - 1;
            for (int i = minSeat / 32 + 1; i < seen.Length; i++)
            {
                uint elem = seen[i];
                if (elem != 0xFFFFFFFFu)
                {
                    int part2 = 32 * i + (31 - BitOperations.LeadingZeroCount(~elem));
                    return new Solution(maxSeat, part2);
                }
            }

            return new Solution(maxSeat, -1);
        }
    }
}
