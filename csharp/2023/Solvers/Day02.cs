using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        var gameId = 1;
        while (input.Length > 1)
        {
            ParseLine(ref input, gameId, out var maxR, out var maxB, out var maxG);

            if (maxR <= 12 && maxG <= 13 && maxB <= 14)
                part1 += gameId;

            part2 += maxR * maxB * maxG;
            gameId++;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static ReadOnlySpan<byte> ParseLine(ref ReadOnlySpan<byte> input, int gameId, out int maxR, out int maxB, out int maxG)
    {
        maxR = 0;
        maxB = 0;
        maxG = 0;

        // skip the "Game 1" part
        input = input[("Game ".Length + (gameId < 10 ? 1 : (gameId < 100 ? 2 : 3)))..];

        while (input[0] != '\n')
        {
            // Parse integer
            byte c;
            var amt = input[2] - '0'; // look at input[2] to skip ": " or ", " or "; "
            var i = 3;
            while ((c = input[i++]) != ' ')
                amt = 10 * amt + (c - '0');

            switch (input[i])
            {
                case (byte)'r':
                    maxR = Math.Max(maxR, amt);
                    input = input[(i + "red".Length)..];
                    break;
                case (byte)'g':
                    maxG = Math.Max(maxG, amt);
                    input = input[(i + "green".Length)..];
                    break;
                case (byte)'b':
                    maxB = Math.Max(maxB, amt);
                    input = input[(i + "blue".Length)..];
                    break;
            }
        }

        input = input[1..];
        return input;
    }
}
