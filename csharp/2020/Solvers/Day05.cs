using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day05 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var seen = new uint[32];
        var numSeats = input.Length / 11;

        var maxSeat = 0;
        for (var i = 0; i < input.Length; i += 11)
        {
            // unrolling the loop to save precious microseconds
            var seatId = 0;
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

        solution.SubmitPart1(maxSeat);

        var minSeat = maxSeat - numSeats - 1;
        for (var i = minSeat / 32 + 1; i < seen.Length; i++)
        {
            var elem = seen[i];
            if (elem != 0xFFFFFFFFu)
            {
                var part2 = 32 * i + (31 - BitOperations.LeadingZeroCount(~elem));
                solution.SubmitPart2(part2);
                return;
            }
        }
    }
}
