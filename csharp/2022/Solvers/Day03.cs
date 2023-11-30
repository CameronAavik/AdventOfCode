using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day03 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int part1 = 0;
        int part2 = 0;

        while (input.Length > 0)
        {
            ReadOnlySpan<byte> sack1 = ReadSack(ref input);
            ReadOnlySpan<byte> sack2 = ReadSack(ref input);
            ReadOnlySpan<byte> sack3 = ReadSack(ref input);

            part1 += GetPriority(sack1, out ulong items1) + GetPriority(sack2, out ulong items2) + GetPriority(sack3, out ulong items3);
            part2 += ExtractPriorityFromBitSet(items1 & items2 & items3);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static ReadOnlySpan<byte> ReadSack(ref ReadOnlySpan<byte> input)
    {
        int sackEndIndex = input.IndexOf((byte)'\n');
        ReadOnlySpan<byte> sack = input[..sackEndIndex];
        input = input[(sackEndIndex + 1)..];
        return sack;
    }

    private static int GetPriority(ReadOnlySpan<byte> rucksack, out ulong sackItems)
    {
        int halfSize = rucksack.Length / 2;
        ulong half1 = 0;
        for (int i = 0; i < halfSize; i++)
            half1 |= 1UL << (rucksack[i] - 'A');

        ulong half2 = 0;
        for (int i = halfSize; i < 2 * halfSize; i++)
            half2 |= 1UL << (rucksack[i] - 'A');

        ulong common = half1 & half2;
        sackItems = half1 | half2;

        return ExtractPriorityFromBitSet(common);
    }

    private static int ExtractPriorityFromBitSet(ulong bits)
    {
        int commonIndex = BitOperations.TrailingZeroCount(bits);
        if (commonIndex < 26)
            return commonIndex + 27;
        else
            return commonIndex - 31;
    }
}
