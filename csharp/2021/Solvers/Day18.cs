using AdventOfCode.CSharp.Common;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day18 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        const int MaxSnailfishCount = 128;
        Span<byte> snailFishes = stackalloc byte[16 * MaxSnailfishCount];
        snailFishes.Fill(255); // 255 indicates that number is empty

        int snailFishCount = ParseAllSnailfish(input, snailFishes);

        Span<byte> snailfishSum = stackalloc byte[16];
        snailFishes.Slice(0, 16).CopyTo(snailfishSum);

        int part2 = 0;
        for (int i = 1; i < snailFishCount; i++)
        {
            // Part 1
            Span<byte> snailFish = snailFishes.Slice(i * 16, 16);
            AddSnailfish(snailfishSum, snailFish);

            // Part 2
            for (int j = 0; j < i; j++)
            {
                Span<byte> otherSnailfish = snailFishes.Slice(j * 16, 16);
                int magnitude1 = AddAndGetMagnitude(snailFish, otherSnailfish);
                if (magnitude1 > part2)
                    part2 = magnitude1;

                int magnitude2 = AddAndGetMagnitude(otherSnailfish, snailFish);
                if (magnitude2 > part2)
                    part2 = magnitude2;
            }
        }

        int part1 = GetMagnitude(snailfishSum);
        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static int AddAndGetMagnitude(Span<byte> fish1, Span<byte> fish2)
    {
        Span<byte> part2SnailfishSum = stackalloc byte[16];
        fish1.CopyTo(part2SnailfishSum);
        AddSnailfish(part2SnailfishSum, fish2);
        return GetMagnitude(part2SnailfishSum);
    }

    private static int ParseAllSnailfish(ReadOnlySpan<byte> input, Span<byte> snailFishes)
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

    private static void ParseSnailfishLine(ReadOnlySpan<byte> input, ref int inputIndex, Span<byte> snailfish)
    {
        byte c = input[inputIndex++];
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
            snailfish[0] = (byte)(c - '0');
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddSnailfish(Span<byte> fish, ReadOnlySpan<byte> fishToAdd)
    {
        AddSnailfishWithExplosions(fish, fishToAdd, out int fishBitset, out int needsSplittingBitset);

        while (needsSplittingBitset != 0)
        {
            // Get index of next number that needs splitting
            // See https://lemire.me/blog/2018/02/21/iterating-over-set-bits-quickly/ for explanation
            int t = needsSplittingBitset & -needsSplittingBitset;
            int indexToSplit = BitOperations.TrailingZeroCount(t);
            needsSplittingBitset ^= t;

            SplitAtIndex(fish, indexToSplit, ref fishBitset, ref needsSplittingBitset);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AddSnailfishWithExplosions(Span<byte> fish, ReadOnlySpan<byte> fishToAdd, out int fishBitset, out int needsSplittingBitset)
    {
        byte carryRight = 0;
        int leftIndex = 255;

        fishBitset = 0;
        needsSplittingBitset = 0;

        // Add first fish
        for (int i = 0; i < 8; i++)
        {
            byte left = fish[i * 2];
            if (left != 255)
            {
                left += carryRight;
                carryRight = 0;

                byte right = fish[i * 2 + 1];
                if (right != 255)
                {
                    if (leftIndex != 255)
                    {
                        int newAmount = fish[leftIndex] += left;
                        if (newAmount >= 10)
                            needsSplittingBitset |= 1 << leftIndex;
                    }

                    left = 0;
                    carryRight = right;
                }

                fishBitset |= 1 << i;
                leftIndex = i;

                if (left >= 10)
                    needsSplittingBitset |= 1 << i;
            }

            fish[i] = left;
        }

        // Add second fish
        for (int i = 0; i < 8; i++)
        {
            int fishIndex = i + 8;
            byte left = fishToAdd[i * 2];
            if (left != 255)
            {
                left += carryRight;
                carryRight = 0;

                byte right = fishToAdd[i * 2 + 1];
                if (right != 255)
                {
                    if (leftIndex != 255)
                    {
                        int newAmount = fish[leftIndex] += left;
                        if (newAmount >= 10)
                            needsSplittingBitset |= 1 << leftIndex;
                    }

                    left = 0;
                    carryRight = right;
                }

                fishBitset |= 1 << fishIndex;
                leftIndex = fishIndex;

                if (left >= 10)
                    needsSplittingBitset |= 1 << fishIndex;
            }

            fish[fishIndex] = left;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SplitAtIndex(Span<byte> fish, int i, ref int fishBitset, ref int needsSplittingBitset)
    {
        byte number = fish[i];
        byte splitLeft = (byte)(number / 2);
        byte splitRight = (byte)(number - splitLeft);

        if ((i & 1) == 1 || (fishBitset & (1 << (i + 1))) != 0)
        {
            // Handle where split causes an explosion
            fish[i] = 0;

            // If the explosion causes a number to the left to go past 10 then we need to set i back to that point
            for (int j = i - 1; j >= 0; j--)
            {
                if ((fishBitset & (1U << j)) != 0)
                {
                    int newAmount = fish[j] += splitLeft;
                    if (newAmount >= 10)
                        needsSplittingBitset |= 1 << j;
                    break;
                }
            }

            for (int j = i + 1; j < fish.Length; j++)
            {
                if ((fishBitset & (1U << j)) != 0)
                {
                    int newAmount = fish[j] += splitRight;
                    if (newAmount >= 10)
                        needsSplittingBitset |= 1 << j;
                    break;
                }
            }
        }
        else
        {
            int distanceToNextFish = 2;
            while ((i & distanceToNextFish) == 0 && (fishBitset & (1 << (i + distanceToNextFish))) == 0)
                distanceToNextFish *= 2;

            fish[i] = splitLeft;
            int splitRightIndex = i + distanceToNextFish / 2;
            fish[splitRightIndex] = splitRight;
            fishBitset |= 1 << splitRightIndex;

            // If the new left value is still greater than 10, we need to keep splitting it
            if (splitLeft >= 10)
                needsSplittingBitset |= 1 << i;

            if (splitRight >= 10)
                needsSplittingBitset |= 1 << splitRightIndex;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetMagnitude(Span<byte> snailFish)
    {
        static int GetMagnitudeInternal(Span<byte> snailFish, int firstNumber, int from, int to)
        {
            if (from + 1 == to)
                return firstNumber;

            int halfLen = from + (to - from) / 2;
            int left = GetMagnitudeInternal(snailFish, firstNumber, from, halfLen);

            int rightStart = snailFish[halfLen];
            return rightStart == 255
                ? left
                : 3 * left + 2 * GetMagnitudeInternal(snailFish, rightStart, halfLen, to);
        }

        return GetMagnitudeInternal(snailFish, snailFish[0], 0, 16);
    }
}
