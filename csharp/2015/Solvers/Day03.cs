using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day03 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var seen = new HashSet<uint>();

        RunSanta(input, seen, start: 0, step: 1);
        int part1 = seen.Count;

        seen.Clear();
        RunSanta(input, seen, start: 0, step: 2);
        RunSanta(input, seen, start: 1, step: 2);
        int part2 = seen.Count;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void RunSanta(ReadOnlySpan<byte> moves, HashSet<uint> seen, int start, int step)
    {
        const int unitX = 1 << 16;
        const int unitY = 1;

        // get starting coordinates 
        uint x = ushort.MaxValue / 2;
        uint y = ushort.MaxValue / 2;

        // pack them into a single uint
        uint encodedPos = x << 16 | y;
        seen.Add(encodedPos);

        for (int i = start; i < moves.Length; i += step)
        {
            switch (moves[i])
            {
                case (byte)'^':
                    encodedPos += unitY;
                    break;
                case (byte)'v':
                    encodedPos -= unitY;
                    break;
                case (byte)'>':
                    encodedPos += unitX;
                    break;
                case (byte)'<':
                    encodedPos -= unitX;
                    break;
            }

            seen.Add(encodedPos);
        }
    }
}
