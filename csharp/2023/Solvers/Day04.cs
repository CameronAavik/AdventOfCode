using System;
using System.Numerics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day04 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int lineLength = input.IndexOf((byte)'\n');
        int numCards = input.Length / (lineLength + 1);
        int cardIdWidth = numCards < 10 ? 1 : (numCards < 100 ? 2 : 3); // can't be bothered to do this properly
        int cardIdStartLen = "Card ".Length + cardIdWidth + ": ".Length;
        int dividerIndex = input.IndexOf((byte)'|');
        int numWinningPerCard = (dividerIndex - cardIdStartLen) / 3; // assumes all numbers are a fixed width of 2
        int numNumbersPerCard = (lineLength - dividerIndex - 1) / 3;

        Span<ulong> winningBitSet = stackalloc ulong[2]; // enough to store a bit set for 100 numbers
        Span<ulong> numbersBitSet = stackalloc ulong[2];
        Span<int> cardCopies = stackalloc int[10];
        int copyIndex = 0;

        int part1 = 0;
        int part2 = 0;

        for (int i = 0; i < numCards; i++)
        {
            winningBitSet[0] = 0;
            winningBitSet[1] = 0;
            numbersBitSet[0] = 0;
            numbersBitSet[1] = 0;

            input = input.Slice(cardIdStartLen);

            // The ' ' character and '0' are 0x20 and 0x30 in ascii respectively, so doing & 0xF will treat them as the same
            for (int j = 0; j < numWinningPerCard; j++)
            {
                int num = 10 * (input[0] & 0xF) + (input[1] & 0xF);
                winningBitSet[num / 64] |= 1UL << num;
                input = input.Slice(3);
            }

            input = input.Slice("| ".Length);

            for (int j = 0; j < numNumbersPerCard; j++)
            {
                int num = 10 * (input[0] & 0xF) + (input[1] & 0xF);
                numbersBitSet[num / 64] |= 1UL << num;
                input = input.Slice(3);
            }

            int matches = BitOperations.PopCount(winningBitSet[0] & numbersBitSet[0]) + BitOperations.PopCount(winningBitSet[1] & numbersBitSet[1]);
            part1 += (1 << matches) >> 1; // This is equivalent to doing 2 ^ (matches - 1) except it handles the case where matches = 0 where the score should be 0

            int copies = cardCopies[copyIndex] + 1;
            cardCopies[copyIndex] = 0;
            part2 += copies;

            for (int j = copyIndex + 1; j < 10 && matches > 0; j++, matches--)
                cardCopies[j] += copies;

            for (int j = 0; j < matches; j++)
                cardCopies[j] += copies;

            copyIndex++;
            if (copyIndex == 10)
                copyIndex = 0;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
