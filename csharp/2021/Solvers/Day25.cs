using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day25 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var width = input.IndexOf((byte)'\n');
        var height = input.Length / (width + 1);

        //int ulongsPerRow = (width + 63) / 64;
        const int ulongsPerRow = 3;
        var countInLastUlong = width % 64;
        var lastUlongMask = (1UL << countInLastUlong) - 1;
        Span<ulong> easts = stackalloc ulong[ulongsPerRow * height];
        Span<ulong> souths = stackalloc ulong[ulongsPerRow * height];

        for (var row = 0; row < height; row++)
        {
            var rowInput = input.Slice(row * (width + 1), width);
            var eastData = easts.Slice(row * ulongsPerRow, ulongsPerRow);
            var southData = souths.Slice(row * ulongsPerRow, ulongsPerRow);
            for (var col = 0; col < width; col++)
            {
                var c = rowInput[col];
                if (c == '>')
                    eastData[col / 64] |= 1UL << (col % 64);
                else if (c == 'v')
                    southData[col / 64] |= 1UL << (col % 64);
            }
        }

        var steps = 0;
        while (true)
        {
            var containsMove = false;

            // move all east-facing cucumbers
            for (var row = 0; row < height; row++)
            {
                var eastData = easts.Slice(row * ulongsPerRow, ulongsPerRow);
                var southData = souths.Slice(row * ulongsPerRow, ulongsPerRow);

                var e1 = eastData[0];
                var e2 = eastData[1];
                var e3 = eastData[2];

                var combined1 = e1 | southData[0];
                var combined2 = e2 | southData[1];
                var combined3 = e3 | southData[2];

                var lastE3 = e3 >> (countInLastUlong - 1);
                e3 = (e3 << 1 | e2 >> 63) & lastUlongMask;
                e2 = e2 << 1 | e1 >> 63;
                e1 = e1 << 1 | lastE3;

                var overlap1 = e1 & combined1;
                var overlap2 = e2 & combined2;
                var overlap3 = e3 & combined3;

                if (overlap1 != e1 || overlap2 != e2 || overlap3 != e3)
                    containsMove = true;

                eastData[0] = (e1 ^ overlap1) | (overlap1 >> 1) | (overlap2 << 63);
                eastData[1] = (e2 ^ overlap2) | (overlap2 >> 1) | (overlap3 << 63);
                eastData[2] = (e3 ^ overlap3) | (overlap3 >> 1) | ((overlap1 & 1) << (countInLastUlong - 1));
            }

            // Cache the last row data so it can be used again for the end
            var lastRowSouthData = souths.Slice((height - 1) * ulongsPerRow, ulongsPerRow);
            var lastRowS1 = lastRowSouthData[0];
            var lastRowS2 = lastRowSouthData[1];
            var lastRowS3 = lastRowSouthData[2];
            lastRowSouthData.Clear();

            // move all south-facing cucumbers
            var nextRowSouthData = lastRowSouthData;
            var nextRowS1 = lastRowS1;
            var nextRowS2 = lastRowS2;
            var nextRowS3 = lastRowS3;
            for (var row = height - 2; row >= 0; row--)
            {
                var nextRowEastData = easts.Slice((row + 1) * ulongsPerRow, ulongsPerRow);
                var southData = souths.Slice(row * ulongsPerRow, ulongsPerRow);

                var s1 = southData[0];
                var s2 = southData[1];
                var s3 = southData[2];

                var overlap1 = s1 & (nextRowEastData[0] | nextRowS1);
                var overlap2 = s2 & (nextRowEastData[1] | nextRowS2);
                var overlap3 = s3 & (nextRowEastData[2] | nextRowS3);

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
                var overlap1 = lastRowS1 & (easts[0] | nextRowS1);
                var overlap2 = lastRowS2 & (easts[1] | nextRowS2);
                var overlap3 = lastRowS3 & (easts[2] | nextRowS3);

                if (overlap1 != lastRowS1 || overlap2 != lastRowS2 || overlap3 != lastRowS3)
                    containsMove = true;

                nextRowSouthData[0] |= lastRowS1 ^ overlap1;
                nextRowSouthData[1] |= lastRowS2 ^ overlap2;
                nextRowSouthData[2] |= lastRowS3 ^ overlap3;

                lastRowSouthData[0] |= overlap1;
                lastRowSouthData[1] |= overlap2;
                lastRowSouthData[2] |= overlap3;
            }

            steps++;

            if (!containsMove)
                break;
        }

        solution.SubmitPart1(steps);
        solution.SubmitPart2(string.Empty);
    }
}
