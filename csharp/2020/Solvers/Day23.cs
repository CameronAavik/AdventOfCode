using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2020.Solvers;

public class Day23 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        input = input.TrimEnd((byte)'\n');
        var part1Cups = new int[input.Length];

        var startingCup = input[0] - '1';
        var prevCup = startingCup;
        foreach (var c in input)
        {
            var digit = c - '1';
            part1Cups[prevCup] = digit;
            prevCup = digit;
        }

        var part2Cups = new int[1000000];
        Array.Copy(part1Cups, part2Cups, part1Cups.Length);

        part2Cups[prevCup] = part1Cups.Length;
        for (var i = part1Cups.Length; i < 1000000; i++)
        {
            part2Cups[i] = i + 1;
        }

        // link final cups back to the starting cup
        part1Cups[prevCup] = startingCup;
        part2Cups[1000000 - 1] = startingCup;

        var cur = startingCup;
        for (var i = 0; i < 100; i++)
        {
            cur = Iterate(part1Cups, cur);
        }

        SolvePart1AndSubmit(part1Cups, solution.GetPart1Writer());

        cur = startingCup;
        for (var i = 0; i < 10000000; i++)
        {
            cur = Iterate(part2Cups, cur);
        }

        var part2 = GetPart2Answer(part2Cups);

        solution.SubmitPart2(part2);

        static void SolvePart1AndSubmit(int[] cups, SolutionWriter solutionWriter)
        {
            var digit = cups[0];
            for (var i = 0; i < cups.Length - 1; i++)
            {
                solutionWriter.Write((char)(digit + '1'));
                digit = cups[digit];
            }

            solutionWriter.Complete();
        }

        static long GetPart2Answer(int[] cups)
        {
            return (long)(cups[0] + 1) * (cups[cups[0]] + 1);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Iterate(int[] cups, int cur)
    {
        var move1 = cups[cur];
        var move2 = cups[move1];
        var move3 = cups[move2];
        var next = cups[move3];
        cups[cur] = next;

        var destination = cur - 1;
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
