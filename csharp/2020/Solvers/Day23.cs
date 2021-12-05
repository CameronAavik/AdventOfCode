using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day23 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        input = input.TrimEnd('\n');
        int[] part1Cups = new int[input.Length];

        int startingCup = input[0] - '1';
        int prevCup = startingCup;
        foreach (char c in input)
        {
            int digit = c - '1';
            part1Cups[prevCup] = digit;
            prevCup = digit;
        }

        int[] part2Cups = new int[1000000];
        Array.Copy(part1Cups, part2Cups, part1Cups.Length);

        part2Cups[prevCup] = part1Cups.Length;
        for (int i = part1Cups.Length; i < 1000000; i++)
        {
            part2Cups[i] = i + 1;
        }

        // link final cups back to the starting cup
        part1Cups[prevCup] = startingCup;
        part2Cups[1000000 - 1] = startingCup;

        int cur = startingCup;
        for (int i = 0; i < 100; i++)
        {
            cur = Iterate(part1Cups, cur);
        }

        string part1 = GetPart1Answer(part1Cups);

        cur = startingCup;
        for (int i = 0; i < 10000000; i++)
        {
            cur = Iterate(part2Cups, cur);
        }

        long part2 = GetPart2Answer(part2Cups);

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);

        static string GetPart1Answer(int[] cups)
        {
            char[] chars = new char[cups.Length - 1];

            int digit = cups[0];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = (char)(digit + '1');
                digit = cups[digit];
            }

            return new string(chars);
        }

        static long GetPart2Answer(int[] cups) => (long)(cups[0] + 1) * (cups[cups[0]] + 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Iterate(int[] cups, int cur)
    {
        int move1 = cups[cur];
        int move2 = cups[move1];
        int move3 = cups[move2];
        int next = cups[move3];
        cups[cur] = next;

        int destination = cur - 1;
        if (destination < 0)
            destination = cups.Length - 1;

        while (destination == move1 || destination == move2 || destination == move3)
        {
            destination--;
            if (destination < 0)
                destination = cups.Length - 1;
        }

        cups[move3] = cups[destination];
        cups[destination] = move1;
        return next;
    }
}
