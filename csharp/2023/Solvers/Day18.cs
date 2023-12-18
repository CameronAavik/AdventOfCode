using System;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day18 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int x1 = 0;
        int y1 = 0;
        long x2 = 0;
        long y2 = 0;

        int part1Boundary = 0;
        int part1InternalDoubled = 0;
        long part2Boundary = 0;
        long part2InternalDoubled = 0;

        while (!input.IsEmpty)
        {
            int lineEndIndex = input.IndexOf((byte)'\n');
            ReadOnlySpan<byte> line = input.Slice(0, lineEndIndex);
            ParseLine(line, out char dir1, out int amt1, out char dir2, out int amt2);

            // Keep track of how many tiles are on the boundary
            part1Boundary += amt1;
            part2Boundary += amt2;

            // Use Shoelace formula to keep track of how many tiles are inside
            switch (dir1)
            {
                case 'R': part1InternalDoubled -= amt1 * y1; x1 += amt1; break;
                case 'L': part1InternalDoubled += amt1 * y1; x1 -= amt1; break;
                case 'D': part1InternalDoubled += amt1 * x1; y1 += amt1; break;
                case 'U': part1InternalDoubled -= amt1 * x1; y1 -= amt1; break;
            }

            switch (dir2)
            {
                case 'R': part2InternalDoubled -= amt2 * y2; x2 += amt2; break;
                case 'L': part2InternalDoubled += amt2 * y2; x2 -= amt2; break;
                case 'D': part2InternalDoubled += amt2 * x2; y2 += amt2; break;
                case 'U': part2InternalDoubled -= amt2 * x2; y2 -= amt2; break;
            }

            input = input.Slice(lineEndIndex + 1);
        }

        // Use Pick's theorem to calculate the total area
        int part1 =  (Math.Abs(part1InternalDoubled) - part1Boundary) / 2 + part1Boundary + 1;
        long part2 = (Math.Abs(part2InternalDoubled) - part2Boundary) / 2 + part2Boundary + 1;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void ParseLine(ReadOnlySpan<byte> line, out char dir1, out int amt1, out char dir2, out int amt2)
    {
        dir1 = (char)line[0];
        amt1 = line.Length == "R 1 (#abcdef)".Length
            ? line[2] - '0'
            : 10 * line[2] + line[3] - ('0' * 10 + '0');

        ReadOnlySpan<byte> hex = line.Slice(line.Length - "0a7a60)".Length, 6);
        amt2 = (int)(ParseHex(hex[0]) << 16 | ParseHex(hex[1]) << 12 | ParseHex(hex[2]) << 8 | ParseHex(hex[3]) << 4 | ParseHex(hex[4]));
        dir2 = "RDLU"[hex[5] - '0'];
    }

    private static uint ParseHex(byte c) => (uint)((c & 0xF) + 9 * (c >> 6));
}
