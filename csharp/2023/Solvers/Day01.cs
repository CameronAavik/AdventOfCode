using System;
using System.Buffers;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day01 : ISolver
{
    private static readonly SearchValues<byte> s_numberStarts = SearchValues.Create("otfsen"u8);
    private static readonly SearchValues<byte> s_numberEnds = SearchValues.Create("eorxnt"u8);

    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int part1 = 0;
        int part2 = 0;

        while (input.Length > 1) // assume file ends with newline
        {
            int lineEndIndex = input.IndexOf((byte)'\n');
            ReadOnlySpan<byte> line = input.Slice(0, lineEndIndex);
            input = input.Slice(lineEndIndex + 1);

            int firstDigitIndex = line.IndexOfAnyInRange((byte)'1', (byte)'9');
            int firstDigit = 10 * (line[firstDigitIndex] - '0');

            int lastDigitIndex = line.LastIndexOfAnyInRange((byte)'1', (byte)'9');
            int lastDigit = line[lastDigitIndex] - '0';

            part1 += firstDigit + lastDigit;

            FindFirstWrittenDigit(line.Slice(0, firstDigitIndex), ref firstDigit);
            FindLastWrittenDigit(line.Slice(lastDigitIndex + 1), ref lastDigit);

            part2 += firstDigit + lastDigit;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void FindFirstWrittenDigit(ReadOnlySpan<byte> span, ref int digit)
    {
        while (span.Length > 2)
        {
            int candidateStartIndex = span.IndexOfAny(s_numberStarts);
            if (candidateStartIndex < 0)
                break;

            span = span.Slice(candidateStartIndex);

            switch (span)
            {
                case [(byte)'o', (byte)'n', (byte)'e', ..]: digit = 10; return;
                case [(byte)'t', (byte)'w', (byte)'o', ..]: digit = 20; return;
                case [(byte)'t', (byte)'h', (byte)'r', (byte)'e', (byte)'e', ..]: digit = 30; return;
                case [(byte)'f', (byte)'o', (byte)'u', (byte)'r', ..]: digit = 40; return;
                case [(byte)'f', (byte)'i', (byte)'v', (byte)'e', ..]: digit = 50; return;
                case [(byte)'s', (byte)'i', (byte)'x', ..]: digit = 60; return;
                case [(byte)'s', (byte)'e', (byte)'v', (byte)'e', (byte)'n', ..]: digit = 70; return;
                case [(byte)'e', (byte)'i', (byte)'g', (byte)'h', (byte)'t', ..]: digit = 80; return;
                case [(byte)'n', (byte)'i', (byte)'n', (byte)'e', ..]: digit = 90; return;
                case { Length: < 4 }: return;
            };

            span = span.Slice(1);
        }
    }

    private static void FindLastWrittenDigit(ReadOnlySpan<byte> span, ref int digit)
    {
        while (span.Length > 2)
        {
            int candidateEndIndex = span.LastIndexOfAny(s_numberEnds);
            if (candidateEndIndex < 0)
                break;

            span = span.Slice(0, candidateEndIndex + 1);

            switch (span)
            {
                case [.., (byte)'o', (byte)'n', (byte)'e']: digit = 1; return;
                case [.., (byte)'t', (byte)'w', (byte)'o']: digit = 2; return;
                case [.., (byte)'t', (byte)'h', (byte)'r', (byte)'e', (byte)'e']: digit = 3; return;
                case [.., (byte)'f', (byte)'o', (byte)'u', (byte)'r']: digit = 4; return;
                case [.., (byte)'f', (byte)'i', (byte)'v', (byte)'e']: digit = 5; return;
                case [.., (byte)'s', (byte)'i', (byte)'x']: digit = 6; return;
                case [.., (byte)'s', (byte)'e', (byte)'v', (byte)'e', (byte)'n']: digit = 7; return;
                case [.., (byte)'e', (byte)'i', (byte)'g', (byte)'h', (byte)'t']: digit = 8; return;
                case [.., (byte)'n', (byte)'i', (byte)'n', (byte)'e']: digit = 9; return;
                case { Length: < 4 }: return;
            };

            span = span.Slice(0, span.Length - 1);
        }
    }
}
