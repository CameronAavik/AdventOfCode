using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day18 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        const int MaxSnailfishCount = 100;
        Span<int> snailFishes = stackalloc int[16 * MaxSnailfishCount];
        snailFishes.Fill(-1); // -1 indicates that number is empty

        int snailFishCount = ParseAllSnailfish(input, snailFishes);

        Span<int> snailfishSum = stackalloc int[16];
        snailFishes.Slice(0, 16).CopyTo(snailfishSum);

        Span<int> part2SnailfishSum = stackalloc int[16];
        int part2 = 0;
        for (int i = 1; i < snailFishCount; i++)
        {
            Span<int> snailFish = snailFishes.Slice(i * 16, 16);
            AddSnailfish(snailfishSum, snailFish);

            for (int j = 0; j < i; j++)
            {
                Span<int> otherSnailfish = snailFishes.Slice(j * 16, 16);
                snailFish.CopyTo(part2SnailfishSum);

                AddSnailfish(part2SnailfishSum, otherSnailfish);
                int magnitude1 = GetMagnitude(part2SnailfishSum);
                if (magnitude1 > part2)
                    part2 = magnitude1;

                otherSnailfish.CopyTo(part2SnailfishSum);

                AddSnailfish(part2SnailfishSum, snailFish);
                int magnitude2 = GetMagnitude(part2SnailfishSum);
                if (magnitude2 > part2)
                    part2 = magnitude2;
            }
        }

        int part1 = GetMagnitude(snailfishSum.Slice(0, 16));
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int ParseAllSnailfish(ReadOnlySpan<char> input, Span<int> snailFishes)
    {
        int snailFishCount = 0;
        int inputIndex = 0;
        while (inputIndex < input.Length)
        {
            ParseSnailfishLine(input, ref inputIndex, snailFishes.Slice(16 * snailFishCount++, 16));
            inputIndex++; // skip newline
        }

        return snailFishCount;
    }

    private static void ParseSnailfishLine(ReadOnlySpan<char> input, ref int inputIndex, Span<int> snailfish)
    {
        char c = input[inputIndex++];
        if (c == '[')
        {
            int halfLen = snailfish.Length / 2;
            ParseSnailfishLine(input, ref inputIndex, snailfish.Slice(0, halfLen));
            inputIndex++; // skip comma
            ParseSnailfishLine(input, ref inputIndex, snailfish.Slice(halfLen));
            inputIndex++; // skip end bracket
        }
        else
        {
            snailfish[0] = c - '0';
        }
    }

    private static void AddSnailfish(Span<int> fish, ReadOnlySpan<int> fishToAdd)
    {
        AddSnailfishWithExplosions(fish, fishToAdd);

        // Do all splits
        int i = 0;
        while (i < fish.Length)
        {
            int number = fish[i];
            if (number is >= 10)
            {
                int splitLeft = number / 2;
                int splitRight = number - splitLeft;

                if ((i & 1) == 1 || fish[i + 1] != -1)
                {
                    // If the split causes an explosion, do it instead
                    fish[i] = 0;

                    // If the explosion causes a number to the left to go past 10 then we need to set i back to that point
                    int? newSplitIndex = null;
                    for (int j = i - 1; j >= 0; j --)
                    {
                        if (fish[j] != -1)
                        {
                            int newAmount = fish[j] += splitLeft;
                            if (newAmount >= 10)
                                newSplitIndex = j;
                            break;
                        }
                    }

                    for (int j = i + 1; j < fish.Length; j++)
                    {
                        if (fish[j] != -1)
                        {
                            fish[j] += splitRight;
                            break;
                        }
                    }

                    if (newSplitIndex is int newI)
                    {
                        i = newI;
                        continue;
                    }
                }
                else
                {
                    int distToNext = 2;
                    while ((i & distToNext) != distToNext && fish[i + distToNext] == -1)
                        distToNext *= 2;

                    fish[i] = splitLeft;
                    fish[i + distToNext / 2] = splitRight;

                    // If the new left value is still greater than 10, we need to keep splitting it
                    if (splitLeft >= 10)
                        continue;
                }
            }

            i++;
        }
    }

    private static void AddSnailfishWithExplosions(Span<int> fish, ReadOnlySpan<int> fishToAdd)
    {
        int carryRight = 0;
        int leftIndex = -1;

        // Add first fish
        for (int i = 0; i < 8; i++)
        {
            int left = fish[i * 2];
            if (left != -1)
            {
                left += carryRight;
                carryRight = 0;

                int right = fish[i * 2 + 1];
                if (right != -1)
                {
                    if (leftIndex != -1)
                        fish[leftIndex] += left;

                    left = 0;
                    carryRight = right;
                }

                leftIndex = i;
            }

            fish[i] = left;
        }

        // Add second fish
        for (int i = 0; i < 8; i++)
        {
            int fishIndex = i + 8;
            int left = fishToAdd[i * 2];
            if (left != -1)
            {
                left += carryRight;
                carryRight = 0;

                int right = fishToAdd[i * 2 + 1];
                if (right != -1)
                {
                    if (leftIndex != -1)
                        fish[leftIndex] += left;

                    left = 0;
                    carryRight = right;
                }

                leftIndex = fishIndex;
            }

            fish[fishIndex] = left;
        }
    }

    private static int GetMagnitude(Span<int> snailFish)
    {
        if (snailFish.Length == 1)
        {
            return snailFish[0];
        }
        else
        {
            int halfLen = snailFish.Length / 2;
            int left = GetMagnitude(snailFish.Slice(0, halfLen));

            return snailFish[halfLen] == -1
                ? left
                : 3 * left + 2 * GetMagnitude(snailFish.Slice(halfLen));
        }
    }
}
