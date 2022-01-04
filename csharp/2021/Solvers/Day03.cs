using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day03 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int bitsPerNumber = input.IndexOf((byte)'\n');
        int lineLength = bitsPerNumber + 1;
        int numbers = input.Length / lineLength;

        // arrLen is the number of 64 bit integers needed to store 'numbers' amount of bits.
        int arrLen = (numbers + 63) / 64;
        int bitsInLastElement = numbers % 64;

        // Build masks where bits represent which rows are still being included when determining the oxygen and CO2 ratings.
        Span<ulong> oxygenRatingMask = arrLen <= 16 ? stackalloc ulong[16] : new ulong[arrLen];
        Span<ulong> co2RatingMask = arrLen <= 16 ? stackalloc ulong[16] : new ulong[arrLen];

        // By default, all rows are included, so set everything to 1 which can be done by filling with ~0UL.
        oxygenRatingMask.Fill(~0UL);

        // If the number of bits in the last element is greater than zero, then ensure that the extra bits are off.
        if (bitsInLastElement > 0)
            oxygenRatingMask[arrLen - 1] = (1UL << bitsInLastElement) - 1UL;

        // Copy Oxygen mask to CO2 mask since they should start off the same
        oxygenRatingMask.CopyTo(co2RatingMask);

        // Keep track of how many numbers are still in consideration for oxygen and CO2
        int oxygenRatingMaskSize = numbers;
        int co2RatingMaskSize = numbers;

        int gammaRate = 0;
        int oxygenRating = 0;
        int co2Rating = 0;

        Span<ulong> onesMask = arrLen <= 16 ? stackalloc ulong[16] : new ulong[arrLen];
        for (int bit = 0; bit < bitsPerNumber; bit++)
        {
            int onesCount = 0;
            int oxygenOnesCount = 0;
            int co2OnesCount = 0;

            for (int i = 0; i < arrLen; i++)
            {
                // We process the input in batches of 64
                ulong onesInBit = GetNext64OnesForBit(input.Slice(i * lineLength * 64), bit, lineLength);
                onesCount += BitOperations.PopCount(onesInBit);
                oxygenOnesCount += BitOperations.PopCount(onesInBit & oxygenRatingMask[i]);
                co2OnesCount += BitOperations.PopCount(onesInBit & co2RatingMask[i]);
                onesMask[i] = onesInBit;
            }

            gammaRate = gammaRate * 2 + (onesCount * 2 >= numbers ? 1 : 0);

            int oxygenBit = oxygenOnesCount * 2 >= oxygenRatingMaskSize ? 1 : 0;
            oxygenRating = oxygenRating * 2 + oxygenBit;

            int co2Bit = co2RatingMaskSize > 1 ? (co2OnesCount * 2 >= co2RatingMaskSize ? 0 : 1) : co2OnesCount;
            co2Rating = co2Rating * 2 + co2Bit;

            oxygenRatingMaskSize = oxygenBit == 1 ? oxygenOnesCount : oxygenRatingMaskSize - oxygenOnesCount;
            co2RatingMaskSize = co2Bit == 1 ? co2OnesCount : co2RatingMaskSize - co2OnesCount;

            for (int i = 0; i < arrLen; i++)
            {
                ulong onesMaskSegment = onesMask[i];
                oxygenRatingMask[i] &= oxygenBit == 1 ? onesMaskSegment : ~onesMaskSegment;
                co2RatingMask[i] &= co2Bit == 1 ? onesMaskSegment : ~onesMaskSegment;
            }
        }

        // Epsilon rate is just the gamma rate with the bits flipped
        int epsilonRate = gammaRate ^ ((1 << bitsPerNumber) - 1);

        int part1 = gammaRate * epsilonRate;
        int part2 = oxygenRating * co2Rating;
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static ulong GetNext64OnesForBit(ReadOnlySpan<byte> inputSegment, int bit, int lineLength)
    {
        ulong onesMask = 0;

        int i = 0;
        for (int j = bit; j < inputSegment.Length && i < 64; j += lineLength)
            onesMask |= (ulong)(inputSegment[j] & 1) << i++;

        return onesMask;
    }
}
