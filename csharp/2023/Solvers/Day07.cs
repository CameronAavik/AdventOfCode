using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day07: ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        Span<byte> cardCounts = stackalloc byte[16];
        int numHands = input.Count((byte)'\n');

        ulong[] handScoresPart1 = new ulong[numHands];
        ulong[] handScoresPart2 = new ulong[numHands];

        for (int handIndex = 0; handIndex < numHands; handIndex++)
        {
            cardCounts.Clear();
            int handValuePart1 = 0;
            int handValuePart2 = 0;
            int maxOfAKind = 0;
            int numCards = 0;
            for (int cardIndex = 0; cardIndex < 5; cardIndex++)
            {
                byte card = input[cardIndex];
                byte cardValue = CardToValue(card);
                byte newCount = ++cardCounts[cardValue];

                if (newCount == 1)
                    numCards++;

                handValuePart1 |= cardValue << (4 * (4 - cardIndex));

                if (card == 'J')
                    cardValue = 1;
                else
                    maxOfAKind = Math.Max(maxOfAKind, newCount);

                handValuePart2 |= cardValue << (4 * (4 - cardIndex));
            }

            byte jCount = cardCounts[11];
            int handScorePart1 = GetHandType(Math.Max(jCount, maxOfAKind), numCards);
            int handScorePart2 = jCount > 0 ? GetHandType(maxOfAKind + jCount, numCards - 1) : handScorePart1;

            uint c;
            uint bid = (uint)(input[6] & 0xF);
            int i = 7;
            while ((c = input[i++]) != '\n')
                bid = bid * 10 + (c & 0xF);

            input = input.Slice(i);

            handScoresPart1[handIndex] = ((ulong)handScorePart1 << 52) | ((ulong)handValuePart1 << 32) | bid;
            handScoresPart2[handIndex] = ((ulong)handScorePart2 << 52) | ((ulong)handValuePart2 << 32) | bid;
        }

        Array.Sort(handScoresPart1);

        int part1 = 0;
        for (int i = 0; i < handScoresPart1.Length; i++)
            part1 += (int)(handScoresPart1[i] & int.MaxValue) * (i + 1);

        Array.Sort(handScoresPart2);

        int part2 = 0;
        for (int i = 0; i < handScoresPart2.Length; i++)
            part2 += (int)(handScoresPart2[i] & int.MaxValue) * (i + 1);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static byte CardToValue(byte card) => card switch
    {
        (byte)'A' => 14,
        (byte)'K' => 13,
        (byte)'Q' => 12,
        (byte)'J' => 11,
        (byte)'T' => 10,
        _ => (byte)(card & 0xF),
    };

    private static int GetHandType(int maxOfAKind, int numCards)
    {
        if (maxOfAKind == 5) return 6; // 5 of a kind
        if (maxOfAKind == 4) return 5; // 4 of a kind
        if (numCards == 2) return 4; // full house
        if (maxOfAKind == 3) return 3; // 3 of a kind
        if (numCards == 3) return 2; // 2 pairs
        if (maxOfAKind == 2) return 1; // 1 pair
        return 0; // high card
    }
}
