using System;
using System.Numerics;
using System.Runtime.CompilerServices;
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

        int[] cardCopies = new int[numCards];
        for (int i = 0; i < numCards; i++)
            cardCopies[i] = 1;

        int part1 = 0;
        int part2 = 0;

        for (int i = 0; i < numCards; i++)
        {
            winningBitSet[0] = 0;
            winningBitSet[1] = 0;
            numbersBitSet[0] = 0;
            numbersBitSet[1] = 0;

            input = input.Slice(cardIdStartLen);

            for (int j = 0; j < numWinningPerCard; j++)
            {
                int num = 10 * (input[0] | 0x10) + input[1] - ('0' * 10 + '0');
                winningBitSet[num / 64] |= 1UL << (num % 64);
                input = input.Slice(3);
            }

            input = input.Slice("| ".Length);

            for (int j = 0; j < numNumbersPerCard; j++)
            {
                int num = 10 * (input[0] | 0x10) + input[1] - ('0' * 10 + '0');
                numbersBitSet[num / 64] |= 1UL << (num % 64);
                input = input.Slice(3);
            }

            var matches = BitOperations.PopCount(winningBitSet[0] & numbersBitSet[0]) + BitOperations.PopCount(winningBitSet[1] & numbersBitSet[1]);
            part1 += 1 << (matches - 1);

            var copies = cardCopies[i];
            part2 += copies;
            for (int j = i + 1; j <= Math.Min(numCards - 1, i + matches); j++)
                cardCopies[j] += copies;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
