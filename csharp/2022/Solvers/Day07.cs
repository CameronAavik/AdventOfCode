using System;
using System.Collections.Generic;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2022.Solvers;

public class Day07 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        Span<int> dirSizes = stackalloc int[32];
        List<int> allDirSizes = new(256);

        // Assume that the first line of input is always "$ cd /"
        input = input["$ cd /\n".Length..];
        var part1 = 0;
        var depth = 0;
        while (input.Length > 1)
        {
            if (input[0] == '$')
            {
                if (input[2] == 'c')
                {
                    input = input["$ cd ".Length..];
                    if (input.StartsWith(".."u8))
                    {
                        var dirSize = dirSizes[depth];
                        dirSizes[depth - 1] += dirSize;
                        if (dirSize <= 100000)
                            part1 += dirSize;
                        allDirSizes.Add(dirSize);
                        depth--;
                        input = input["..\n".Length..];
                    }
                    else
                    {
                        dirSizes[++depth] = 0;
                        input = input[(input.IndexOf((byte)'\n') + 1)..];
                    }
                }
                else
                {
                    input = input["$ ls\n".Length..];
                }
            }
            else if (input[0] == 'd')
            {
                // no point processing dir names, skip line
                input = input[(input.IndexOf((byte)'\n') + 1)..];
            }
            else
            {
                var sizeEndIndex = input.IndexOf((byte)' ');
                var size = input[0] - '0';
                for (var i = 1; i < sizeEndIndex; i++)
                    size = size * 10 + input[i] - '0';
                dirSizes[depth] += size;
                input = input[(input.IndexOf((byte)'\n') + 1)..];
            }
        }

        var totalDirSize = 0;
        for (var i = depth; i >= 0; i--)
        {
            totalDirSize += dirSizes[i];
            allDirSizes.Add(totalDirSize);
        }

        var amountToFree = totalDirSize - 40000000;
        var part2 = int.MaxValue;
        foreach (var dirSize in allDirSizes)
        {
            if (dirSize > amountToFree && dirSize < part2)
                part2 = dirSize;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
