using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day15 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var width = input.IndexOf((byte)'\n');
        var height = input.Length / (width + 1);

        // The risk level for (x, y) is stored at riskLevels[y * width + x].
        // The risk levels are subtracted by 1 so that they are from the range 0 - 8 instead of 1 - 9
        Span<byte> riskLevels = stackalloc byte[width * height];
        ParseRiskLevels(input, width, height, riskLevels);

        int part1 = FindShortestPath(riskLevels, width, height);

        // Expand the risk levels span to repeat 5 times over.
        int part2Width = 5 * width;
        int part2Height = 5 * height;
        Span<byte> riskLevelsPart2 = stackalloc byte[part2Width * part2Height];
        for (int yRepeat = 0; yRepeat < 5; yRepeat++)
        {
            int yOffset = yRepeat * height;
            for (int xRepeat = 0; xRepeat < 5; xRepeat++)
            {
                int xOffset = xRepeat * width;
                int increase = yRepeat + xRepeat;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int origRiskLevel = riskLevels[y * width + x];
                        int riskAfterIncrease = origRiskLevel + increase;
                        int riskAfterWrapping = riskAfterIncrease % 9;
                        riskLevelsPart2[(yOffset + y) * part2Width + xOffset + x] = (byte)riskAfterWrapping;
                    }
                }
            }
        }

        int part2 = FindShortestPath(riskLevelsPart2, part2Width, part2Height);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int FindShortestPath(Span<byte> riskLevels, int width, int height)
    {
        const int maxBucketSize = 4096;
        Span<byte> seenCells = stackalloc byte[width * height];

        int startBucketPtr = 0;
        int minBucketDistance = 0;
        Span<int> pq = stackalloc int[maxBucketSize * 11];
        Span<int> bucketPtrs = stackalloc int[11];
        for (int i = 0; i < 11; i++)
            bucketPtrs[i] = i * maxBucketSize;

        pq[bucketPtrs[0]++] = 0;
        minBucketDistance = width + height - 2; // heuristic distance to end

        int bottomRightPos = (width * height) - 1;

        while (true)
        {
            for (int i = startBucketPtr; i < bucketPtrs[0]; i++)
            {
                int packedPos = pq[i];
                if (packedPos == bottomRightPos)
                    return minBucketDistance;

                ref byte seenCell = ref seenCells[packedPos];
                if (seenCell != 0)
                    continue;

                seenCell = 1;

                int x = packedPos % width;

                if (x > 0)
                {
                    int riskLevel = riskLevels[packedPos - 1];
                    int bucketPtr = bucketPtrs[riskLevel + 2]++;
                    pq[bucketPtr] = packedPos - 1;
                }

                if (x < width - 1)
                {
                    int riskLevel = riskLevels[packedPos + 1];
                    int bucketPtr = bucketPtrs[riskLevel]++;
                    pq[bucketPtr] = packedPos + 1;
                }

                if (packedPos >= width)
                {
                    int riskLevel = riskLevels[packedPos - width];
                    int bucketPtr = bucketPtrs[riskLevel + 2]++;
                    pq[bucketPtr] = packedPos - width;
                }

                if (packedPos <= bottomRightPos - width)
                {
                    int riskLevel = riskLevels[packedPos + width];
                    int bucketPtr = bucketPtrs[riskLevel]++;
                    pq[bucketPtr] = packedPos + width;
                }
            }

            for (int i = 0; i < 10; i++)
                bucketPtrs[i] = bucketPtrs[i + 1];

            bucketPtrs[10] = startBucketPtr;
            startBucketPtr = (startBucketPtr + maxBucketSize) % (11 * maxBucketSize);
            minBucketDistance++;
        }
    }

    private static void ParseRiskLevels(ReadOnlySpan<byte> input, int width, int height, Span<byte> riskLevels)
    {
        int riskLevelIndex = 0;
        int inputIndex = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                riskLevels[riskLevelIndex++] = (byte)(input[inputIndex++] - '1');
            }

            inputIndex++;
        }
    }
}
