using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day03 : ISolver
{
    public Solution Solve(ReadOnlySpan<char> input)
    {
        int bitsPerNumber = input.IndexOf('\n');
        int numbers = input.Length / (bitsPerNumber + 1);

        (int arrLen, int remainder) = Math.DivRem(numbers, 64);
        if (remainder > 0)
            arrLen++;

        // Build masks where bits represent which rows are still being included when determining the oxygen and CO2 ratings.
        Span<ulong> oxygenRatingMask = stackalloc ulong[arrLen];
        oxygenRatingMask.Fill(~0UL);
        if (remainder > 0)
            oxygenRatingMask[arrLen - 1] = (1UL << remainder) - 1UL;

        Span<ulong> co2RatingMask = stackalloc ulong[arrLen];
        oxygenRatingMask.CopyTo(co2RatingMask);

        int oxygenRatingMaskSize = numbers;
        int co2RatingMaskSize = numbers;

        Span<ulong> onesMask = stackalloc ulong[arrLen];

        int gammaRate = 0;
        int oxygenRating = 0;
        int co2Rating = 0;
        for (int bit = 0; bit < bitsPerNumber; bit++)
        {
            int onesCount = 0;
            int oxygenOnesCount = 0;
            int co2OnesCount = 0;

            for (int i = 0; i < arrLen; i++)
            {
                ulong oxygenRatingMaskSegment = oxygenRatingMask[i];
                ulong co2RatingMaskSegment = co2RatingMask[i];
                ulong onesMaskSegment = 0;

                int cursorOffset = i * (bitsPerNumber + 1) * 64 + bit;

                ulong flag = 1;
                int maxJ = Math.Min(cursorOffset + (bitsPerNumber + 1) * 64, input.Length);
                for (int j = cursorOffset; j < maxJ; j += bitsPerNumber + 1)
                {
                    int oneBit = input[j] & 1;

                    onesCount += oneBit;
                    onesMaskSegment |= flag * (ulong)oneBit;
                    oxygenOnesCount += (int)(oxygenRatingMaskSegment & (ulong)oneBit);
                    co2OnesCount += (int)(co2RatingMaskSegment & (ulong)oneBit);

                    flag <<= 1;
                    oxygenRatingMaskSegment >>= 1;
                    co2RatingMaskSegment >>= 1;
                }

                onesMask[i] = onesMaskSegment;
            }

            gammaRate = gammaRate * 2 + (onesCount * 2 >= numbers ? 1 : 0);

            int oxygenBit = oxygenOnesCount * 2 >= oxygenRatingMaskSize ? 1 : 0;
            int co2Bit = co2RatingMaskSize > 1 ? (co2OnesCount * 2 >= co2RatingMaskSize ? 0 : 1) : co2OnesCount;

            oxygenRating = oxygenRating * 2 + oxygenBit;
            co2Rating = co2Rating * 2 + co2Bit;

            oxygenRatingMaskSize = oxygenBit == 1 ? oxygenOnesCount : oxygenRatingMaskSize - oxygenOnesCount;
            co2RatingMaskSize = co2Bit == 1 ? co2OnesCount : co2RatingMaskSize - co2OnesCount;

            /** WIP Vectorization, not much benefit because there are only 16 array elements in practice.
            
            var lastBlockIndex = arrLen - (arrLen % Vector<ulong>.Count);

            for (int i = 0; i < lastBlockIndex; i += Vector<ulong>.Count)
            {
                var vOnesMask = new Vector<ulong>(onesMask.Slice(i));
                var vOxygenMask = new Vector<ulong>(oxygenRatingMask.Slice(i));
                var vCO2Mask = new Vector<ulong>(co2RatingMask.Slice(i));

                vOxygenMask &= oxygenBit == 1 ? vOnesMask : ~vOnesMask;
                vCO2Mask &= co2Bit == 1 ? vOnesMask : ~vOnesMask;

                vOxygenMask.CopyTo(oxygenRatingMask.Slice(i));
                vCO2Mask.CopyTo(co2RatingMask.Slice(i));
            }

            **/

            for (int i = 0; i < arrLen; i++)
            {
                ulong onesMaskSegment = onesMask[i];
                oxygenRatingMask[i] &= oxygenBit == 1 ? onesMaskSegment : ~onesMaskSegment;
                co2RatingMask[i] &= co2Bit == 1 ? onesMaskSegment : ~onesMaskSegment;
            }
        }

        int epsilonRate = gammaRate ^ ((1 << bitsPerNumber) - 1);
        int part1 = gammaRate * epsilonRate;
        int part2 = oxygenRating * co2Rating;
        return new Solution(part1, part2);
    }
}
