using System;
using System.Buffers.Binary;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2024.Solvers;

public class Day05 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // for all possible values store a 128-bit set of the values that must come before it
        Span<ulong> contingencies = stackalloc ulong[2 * 100];

        int i = 0;
        while (true)
        {
            ulong orderBytes = BinaryPrimitives.ReadUInt64LittleEndian(input.Slice(i)) & 0x0F0F000F0FU; // 12|34\n56 becomes 0x.4.3...2.1

            // When it switches from the rules to the updates, we will see orderBytes represent the ASCII string "\n\n12,34,5"
            // This means that there is now a comma (0x2C) where the first digit of the right number is expected.
            // So we can use this to tell when the rules section is completed
            if ((orderBytes & 0xF000000) == 0xC000000)
                break;

            orderBytes *= 2561; // 0x.4.3...2.1 becomes [_, _, _, 34, _, _, 12, _]
            int left = (int)((orderBytes >> 8) & 0xFF); // Extracts 12 from above array
            int right = (int)((orderBytes >> 32) & 0xFF); // Extracts 34 from above array
            contingencies[2 * right + left / 64] |= 1UL << (left % 64);
            i += 6;
        }

        int part1 = 0;
        int part2 = 0;

        Span<byte> numberOrder = stackalloc byte[100];
        Span<ulong> numberBits = stackalloc ulong[2];
        int numberCount = 0;
        while (i + 3 < input.Length)
        {
            // read next 2 digits starting at the previous separator, so this might get the ASCII bytes for ",29,"
            uint bytes = BinaryPrimitives.ReadUInt32LittleEndian(input.Slice(i)) & 0x0F0F0F0F;
            int number = (int)(((bytes * 2561) >> 16) & 0xFF);
            numberBits[number / 64] |= 1UL << (number % 64);
            numberOrder[numberCount++] = (byte)number;

            if (bytes >> 24 == (byte)'\n')
            {
                bool inOrder = true;
                int medianIndex = numberCount / 2;
                int medianValue = 0;
                for (int j = 0; j < numberCount; j++)
                {
                    number = numberOrder[j];
                    int expectedOrder = BitOperations.PopCount(numberBits[0] & contingencies[number * 2]) + BitOperations.PopCount(numberBits[1] & contingencies[number * 2 + 1]);

                    inOrder = inOrder && expectedOrder == j;
                    if (expectedOrder == medianIndex)
                        medianValue = number;
                }

                if (inOrder)
                    part1 += medianValue;
                else
                    part2 += medianValue;

                numberBits.Clear();
                numberCount = 0;
            }

            i += 3;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
