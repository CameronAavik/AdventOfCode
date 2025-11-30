using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day04 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var lineLength = input.IndexOf((byte)'\n');
        var numCards = input.Length / (lineLength + 1);
        var cardIdWidth = numCards < 10 ? 1 : (numCards < 100 ? 2 : 3); // can't be bothered to do this properly
        var cardIdStartLen = "Card ".Length + cardIdWidth + ": ".Length;
        var dividerIndex = input.IndexOf((byte)'|');
        var numWinningPerCard = (dividerIndex - cardIdStartLen) / 3; // assumes all numbers are a fixed width of 2
        var numNumbersPerCard = (lineLength - dividerIndex - 1) / 3;

        Span<ulong> winningBitSet = stackalloc ulong[2]; // enough to store a bit set for 100 numbers
        Span<ulong> numbersBitSet = stackalloc ulong[2];
        Span<int> cardCopies = stackalloc int[10];
        var copyIndex = 0;

        var part1 = 0;
        var part2 = 0;

        for (var i = 0; i < numCards; i++)
        {
            winningBitSet[0] = 0;
            winningBitSet[1] = 0;
            numbersBitSet[0] = 0;
            numbersBitSet[1] = 0;

            input = input[cardIdStartLen..];

            // The ' ' character and '0' are 0x20 and 0x30 in ascii respectively, so doing & 0xF will treat them as the same
            for (var j = 0; j < numWinningPerCard; j++)
            {
                var num = 10 * (input[0] & 0xF) + (input[1] & 0xF);
                winningBitSet[num / 64] |= 1UL << num;
                input = input[3..];
            }

            input = input["| ".Length..];

            for (var j = 0; j < numNumbersPerCard; j++)
            {
                var num = 10 * (input[0] & 0xF) + (input[1] & 0xF);
                numbersBitSet[num / 64] |= 1UL << num;
                input = input[3..];
            }

            var matches = BitOperations.PopCount(winningBitSet[0] & numbersBitSet[0]) + BitOperations.PopCount(winningBitSet[1] & numbersBitSet[1]);
            part1 += (1 << matches) >> 1; // This is equivalent to doing 2 ^ (matches - 1) except it handles the case where matches = 0 where the score should be 0

            var copies = cardCopies[copyIndex] + 1;
            cardCopies[copyIndex] = 0;
            part2 += copies;

            for (var j = copyIndex + 1; j < 10 && matches > 0; j++, matches--)
                cardCopies[j] += copies;

            for (var j = 0; j < matches; j++)
                cardCopies[j] += copies;

            copyIndex++;
            if (copyIndex == 10)
                copyIndex = 0;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
