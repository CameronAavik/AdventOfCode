using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day05 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        const int MaxCrateHeight = 64;

        var rowLength = input.IndexOf((byte)'\n') + 1;
        var numStacks = rowLength / 4;
        var heights = new int[numStacks];
        var part1Crates = new byte[numStacks][];
        var part2Crates = new byte[numStacks][];
        for (var i = 0; i < numStacks; i++)
        {
            part1Crates[i] = new byte[MaxCrateHeight];
            part2Crates[i] = new byte[MaxCrateHeight];
        }

        // Find the 1 column label index and use that to calculate the stack height
        var maxCratesPerStack = (input.IndexOf((byte)'1') - 1) / rowLength;
        for (var i = 0; i < maxCratesPerStack; i++)
        {
            var rowStart = (maxCratesPerStack - i - 1) * rowLength;
            for (var j = 0; j < numStacks; j++)
            {
                var c = input[rowStart + 4 * j + 1];
                if (c != ' ')
                {
                    heights[j] = i + 1;
                    part1Crates[j][i] = c;
                    part2Crates[j][i] = c;
                }
            }
        }

        var inputPtr = rowLength * (maxCratesPerStack + 1) + 1;
        while (inputPtr < input.Length - 1)
        {
            inputPtr += "move ".Length;
            var numCrates = ReadIntegerUntil(input, ' ', ref inputPtr);
            inputPtr += "from ".Length;
            var fromCrate = ReadIntegerUntil(input, ' ', ref inputPtr) - 1;
            inputPtr += "to ".Length;
            var toCrate = ReadIntegerUntil(input, '\n', ref inputPtr) - 1;

            var fromHeight = heights[fromCrate];
            var toHeight = heights[toCrate];

            for (var i = 0; i < numCrates; i++)
            {
                part1Crates[toCrate][toHeight + i] = part1Crates[fromCrate][fromHeight - i - 1];
                part2Crates[toCrate][toHeight + numCrates - i - 1] = part2Crates[fromCrate][fromHeight - i - 1];
            }

            heights[fromCrate] -= numCrates;
            heights[toCrate] += numCrates;
        }

        var part1Writer = solution.GetPart1Writer();
        var part2Writer = solution.GetPart2Writer();

        for (var i = 0; i < numStacks; i++)
        {
            part1Writer.Write((char)part1Crates[i][heights[i] - 1]);
            part2Writer.Write((char)part2Crates[i][heights[i] - 1]);
        }

        part1Writer.Complete();
        part2Writer.Complete();
    }

    public static int ReadIntegerUntil(ReadOnlySpan<byte> span, char c, ref int i)
    {
        // Assume that the first character is always a digit
        var ret = span[i++] - '0';

        byte cur;
        while ((cur = span[i++]) != c)
            ret = ret * 10 + (cur - '0');

        return ret;
    }
}
