using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day25 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        int width = input.IndexOf('\n');
        int height = input.Length / (width + 1);

        //int ulongsPerRow = (width + 63) / 64;
        const int ulongsPerRow = 3;
        int countInLastUlong = width % 64;
        ulong lastUlongMask = (1UL << countInLastUlong) - 1;
        Span<ulong> easts = stackalloc ulong[ulongsPerRow * height];
        Span<ulong> souths = stackalloc ulong[ulongsPerRow * height];

        for (int row = 0; row < height; row++)
        {
            ReadOnlySpan<char> rowInput = input.Slice(row * width + 1, width);
            Span<ulong> eastData = easts.Slice(row * ulongsPerRow, ulongsPerRow);
            Span<ulong> southData = souths.Slice(row * ulongsPerRow, ulongsPerRow);
            for (int col = 0; col < width; col++)
            {
                char c = rowInput[col];
                if (c == '>')
                    eastData[col / 64] |= 1UL << (col % 64);
                else if (c == 'v')
                    southData[col / 64] |= 1UL << (col % 64);
            }
        }

        int steps = 0;
        while (true)
        {
            bool containsMove = false;

            // move all east-facing cucumbers
            for (int row = 0; row < height; row++)
            {
                Span<ulong> eastData = easts.Slice(row * ulongsPerRow, ulongsPerRow);
                Span<ulong> southData = souths.Slice(row * ulongsPerRow, ulongsPerRow);

                ulong e1 = eastData[0];
                ulong e2 = eastData[1];
                ulong e3 = eastData[2];

                ulong combined1 = e1 | southData[0];
                ulong combined2 = e2 | southData[1];
                ulong combined3 = e3 | southData[2];

                ulong lastE3 = e3 >> (countInLastUlong - 1);
                e3 = (e3 << 1 | e2 >> 63) & lastUlongMask;
                e2 = e2 << 1 | e1 >> 63;
                e1 = e1 << 1 | lastE3;

                ulong overlap1 = e1 & combined1;
                ulong overlap2 = e2 & combined2;
                ulong overlap3 = e3 & combined3;

                if (overlap1 != e1 || overlap2 != e2 || overlap3 != e3)
                    containsMove = true;

                eastData[0] = (e1 ^ overlap1) | (overlap1 >> 1) | (overlap2 << 63);
                eastData[1] = (e2 ^ overlap2) | (overlap2 >> 1) | (overlap3 << 63);
                eastData[2] = (e3 ^ overlap3) | (overlap3 >> 1) | ((overlap1 & 1) << (countInLastUlong - 1));
            }

            // Cache the last row data so it can be used again for the end
            Span<ulong> lastRowSouthData = souths.Slice((height - 1) * ulongsPerRow, ulongsPerRow);
            ulong lastRowS1 = lastRowSouthData[0];
            ulong lastRowS2 = lastRowSouthData[1];
            ulong lastRowS3 = lastRowSouthData[2];

            // move all south-facing cucumbers
            ulong nextRowS1 = lastRowS1;
            ulong nextRowS2 = lastRowS2;
            ulong nextRowS3 = lastRowS3;
            Span<ulong> nextRowSouthData = lastRowSouthData;
            for (int row = height - 2; row > 0; row--)
            {
                Span<ulong> nextRowEastData = easts.Slice((row + 1) * ulongsPerRow, ulongsPerRow);
                Span<ulong> southData = souths.Slice(row * ulongsPerRow, ulongsPerRow);

                ulong s1 = southData[0];
                ulong s2 = southData[1];
                ulong s3 = southData[2];

                ulong overlap1 = s1 & (nextRowEastData[0] | nextRowS1);
                ulong overlap2 = s2 & (nextRowEastData[1] | nextRowS2);
                ulong overlap3 = s3 & (nextRowEastData[2] | nextRowS3);

                if (overlap1 != s1 || overlap2 != s2 || overlap3 != s3)
                    containsMove = true;

                nextRowSouthData[0] |= s1 ^ overlap1;
                nextRowSouthData[1] |= s2 ^ overlap2;
                nextRowSouthData[2] |= s3 ^ overlap3;

                southData[0] = overlap1;
                southData[1] = overlap2;
                southData[2] = overlap3;

                nextRowS1 = s1;
                nextRowS2 = s2;
                nextRowS3 = s3;
                nextRowSouthData = southData;
            }

            // Fix the last row
            {
                ulong overlap1 = lastRowS1 & (easts[0] | nextRowS1);
                ulong overlap2 = lastRowS2 & (easts[1] | nextRowS2);
                ulong overlap3 = lastRowS3 & (easts[2] | nextRowS3);

                if (overlap1 != lastRowS1 || overlap2 != lastRowS2 || overlap3 != lastRowS3)
                    containsMove = true;

                nextRowSouthData[0] |= lastRowS1 ^ overlap1;
                nextRowSouthData[1] |= lastRowS2 ^ overlap2;
                nextRowSouthData[2] |= lastRowS3 ^ overlap3;

                lastRowSouthData[0] |= overlap1;
                lastRowSouthData[1] |= overlap2;
                lastRowSouthData[2] |= overlap3;
            }


            if (!containsMove)
                break;

            steps++;
        }

        solution.SubmitPart1(steps);
        solution.SubmitPart2(string.Empty);
    }
}
