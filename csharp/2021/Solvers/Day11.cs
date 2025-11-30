using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day11 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // Each row of 10 octopi is represented by a 64-bit value. Each octopus is stored using 6 bits, totalling to 60
        // bits and leaving 4 unused bits. The top bit is 1 if the octopus has not flashed in the current step. The
        // next 5 bits are the energy level + 6. The reason 6 is added is because it means that once the energy level
        // hits 10, the second highest bit will be 1, making it easy to tell if an octopus can be flashed.
        Span<long> rows = stackalloc long[10];

        var cursor = 0;
        for (var i = 0; i < 10; i++)
        {
            long row = 0;
            for (var j = 0; j < 10; j++)
            {
                row <<= 6; // Shift the current row up by 6 bits, leaving the bottom 6 bits for the new octopus.
                row |= 1 << 5; // set flag indicating that the octopus has not flashed yet.
                row += input[cursor++] - '0' + 6; // add the energy level + 6.
            }

            rows[i] = row;
            cursor++;
        }

        var totalFlashes = 0;
        for (var step = 0; step < 100; step++)
            totalFlashes += Step(rows);

        solution.SubmitPart1(totalFlashes);

        var step2 = 100;
        while (Step(rows) != 100)
            step2++;

        solution.SubmitPart2(step2 + 1);
    }

    private static int Step(Span<long> rows)
    {
        const long octopusLowestBitMask = 0b000001_000001_000001_000001_000001_000001_000001_000001_000001_000001;
        const long flashResetMask = octopusLowestBitMask * 0b100111; // Set energy level to 1 (stored as 7) and set the unflashed flag.
        const long canBeFlashedFlagMask = octopusLowestBitMask << 4;
        const long isUnflashedFlagMask = octopusLowestBitMask << 5;
        const long bottomNineOctopiMask = (1L << 54) - 1;

        for (var i = 0; i < 10; i++)
        {
            var row = rows[i];

            // Mask for octopi that still have the unflashed flag set
            var unflashedOctopiMask = ((row & isUnflashedFlagMask) >> 5) * 0b111111;
            var flashedOctopiMask = ~unflashedOctopiMask;

            // For each unflashed octopus, add 1 to their energy level
            var unflashedOctopiWithLevelIncrease = unflashedOctopiMask & (row + octopusLowestBitMask);

            // For each flashed octopus, set the energy level to 1 (7 after offset by 6) and with the unflashed bit flag set back to 1.
            var flashedOctopiAfterReest = flashedOctopiMask & flashResetMask;

            rows[i] = unflashedOctopiWithLevelIncrease | flashedOctopiAfterReest;
        }

        var stepFlashes = 0;
        int flashes;
        do
        {
            flashes = 0;

            // Handle first row
            var firstRow = rows[0];

            // Get a 1 in the lowest bit for each octopus that can be flashed
            var firstRowCanBeFlashed = (firstRow & canBeFlashedFlagMask) >> 4;
            flashes += BitOperations.PopCount((ulong)firstRowCanBeFlashed);

            // Set flashed octopi to 0
            firstRow &= ~(firstRowCanBeFlashed * 0b111111);

            // Increment to the left and right
            firstRow += firstRowCanBeFlashed >> 6;
            firstRow += (firstRowCanBeFlashed & bottomNineOctopiMask) << 6;

            var prevRow = firstRow;
            var prevRowFlashed = firstRowCanBeFlashed;

            for (var i = 1; i < 10; i++)
            {
                var row = rows[i];

                // Handle flashes from previous row
                row += prevRowFlashed;
                row += prevRowFlashed >> 6;
                row += (prevRowFlashed & bottomNineOctopiMask) << 6;

                // Get a 1 in the lowest bit for each octopus that can be flashed
                var canBeFlashed = (row & canBeFlashedFlagMask) >> 4;

                if (canBeFlashed > 0)
                {
                    flashes += BitOperations.PopCount((ulong)canBeFlashed);

                    // Set flashed octopi to 0
                    row &= ~(canBeFlashed * 0b111111);

                    // Increment the previous row
                    prevRow += canBeFlashed;
                    prevRow += canBeFlashed >> 6;
                    prevRow += (canBeFlashed & bottomNineOctopiMask) << 6;

                    // Increment to the left and right
                    row += canBeFlashed >> 6;
                    row += (canBeFlashed & bottomNineOctopiMask) << 6;
                }

                rows[i - 1] = prevRow;
                prevRow = row;
                prevRowFlashed = canBeFlashed;
            }

            rows[9] = prevRow;
            stepFlashes += flashes;
        }
        while (flashes > 0);

        return stepFlashes;
    }
}
