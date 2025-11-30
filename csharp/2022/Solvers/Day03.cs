using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day03 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        while (input.Length > 0)
        {
            var sack1 = ReadSack(ref input);
            var sack2 = ReadSack(ref input);
            var sack3 = ReadSack(ref input);

            part1 += GetPriority(sack1, out var items1) + GetPriority(sack2, out var items2) + GetPriority(sack3, out var items3);
            part2 += ExtractPriorityFromBitSet(items1 & items2 & items3);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static ReadOnlySpan<byte> ReadSack(ref ReadOnlySpan<byte> input)
    {
        var sackEndIndex = input.IndexOf((byte)'\n');
        var sack = input[..sackEndIndex];
        input = input[(sackEndIndex + 1)..];
        return sack;
    }

    private static int GetPriority(ReadOnlySpan<byte> rucksack, out ulong sackItems)
    {
        var halfSize = rucksack.Length / 2;
        ulong half1 = 0;
        for (var i = 0; i < halfSize; i++)
            half1 |= 1UL << (rucksack[i] - 'A');

        ulong half2 = 0;
        for (var i = halfSize; i < 2 * halfSize; i++)
            half2 |= 1UL << (rucksack[i] - 'A');

        var common = half1 & half2;
        sackItems = half1 | half2;

        return ExtractPriorityFromBitSet(common);
    }

    private static int ExtractPriorityFromBitSet(ulong bits)
    {
        var commonIndex = BitOperations.TrailingZeroCount(bits);
        if (commonIndex < 26)
            return commonIndex + 27;
        else
            return commonIndex - 31;
    }
}
