﻿using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day02 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        const string part1Code = "123456789";
        const string part2Code = "0010002340567890ABC000E00";

        int part1x = 1, part1y = 1;
        int part2x = 2, part2y = 2;

        SolutionWriter part1Writer = solution.GetPart1Writer();
        SolutionWriter part2Writer = solution.GetPart2Writer();

        foreach (Range lineRange in input.SplitLines())
        {
            ReadOnlySpan<byte> line = input[lineRange];
            foreach (byte dir in line)
            {
                switch (dir)
                {
                    case (byte)'L':
                        part1x = Math.Max(part1x - 1, 0);
                        part2x = Math.Max(part2x - 1, Math.Abs(2 - part2y));
                        break;
                    case (byte)'R':
                        part1x = Math.Min(part1x + 1, 2);
                        part2x = Math.Min(part2x + 1, 4 - Math.Abs(2 - part2y));
                        break;
                    case (byte)'U':
                        part1y = Math.Max(part1y - 1, 0);
                        part2y = Math.Max(part2y - 1, Math.Abs(2 - part2x));
                        break;
                    case (byte)'D':
                        part1y = Math.Min(part1y + 1, 2);
                        part2y = Math.Min(part2y + 1, 4 - Math.Abs(2 - part2x));
                        break;
                }
            }

            part1Writer.Write(part1Code[part1x + 3 * part1y]);
            part2Writer.Write(part2Code[part2x + 5 * part2y]);
        }

        // Put a newline at the end to signify end of output
        part1Writer.Complete();
        part2Writer.Complete();
    }
}
