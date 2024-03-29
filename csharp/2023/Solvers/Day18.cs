﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day18 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int y1 = 0;
        long y2 = 0;
        int perimeter1 = 0;
        int area1 = 0;
        long perimeter2 = 0;
        long area2 = 0;

        while (!input.IsEmpty)
        {
            byte dir1 = input[0];

            int offset = input[3] == ' ' ? 0 : 1; // gets compiled to branchless version

            input = input.Slice(offset + 1);
            int len1 = ReadTwoDigitInteger(input);

            switch (dir1)
            {
                case (byte)'R': area1 += len1 * y1; break;
                case (byte)'D': y1 += len1; break;
                case (byte)'L': area1 -= len1 * y1; break;
                case (byte)'U': y1 -= len1; break;
            }

            perimeter1 += len1;

            input = input.Slice(5);

            long len2 = ReadPartTwoData(input, out int dir2);

            // Keep track of how many tiles are on the boundary
            perimeter2 += len2;

            // Use Shoelace formula to keep track of how many tiles are inside
            switch (dir2)
            {
                case '0': area2 += len2 * y2; break;
                case '1': y2 += len2; break;
                case '2': area2 -= len2 * y2; break;
                case '3': y2 -= len2; break;
            }

            input = input.Slice(8);
        }

        // Use Pick's theorem to calculate the total area
        int part1 = Math.Abs(area1) + perimeter1 / 2 + 1;
        long part2 = Math.Abs(area2) + perimeter2 / 2 + 1;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);

        static int ReadTwoDigitInteger(ReadOnlySpan<byte> input)
        {
            int len1Bits = Unsafe.As<byte, ushort>(ref MemoryMarshal.GetReference(input)) & 0xF0F;
            return ((len1Bits >> 8) + len1Bits * 10) & 0xFF;
        }

        static long ReadPartTwoData(ReadOnlySpan<byte> input, out int dir)
        {
            ulong hexBits = Unsafe.As<byte, ulong>(ref MemoryMarshal.GetReference(input)); // read 8 bytes into a 64 bit integer starting at hex
            dir = (int)((hexBits >> 40) & 0xFF);

            ulong len2Bits = (hexBits & 0x0F0F0F0F0FUL) + 9 * ((hexBits >> 6) & 0x0101010101UL); // byte order is _0_1_2_3_4
            len2Bits = (len2Bits << 12 | len2Bits) & 0xF000FF00FF00; // byte order is now 0___21__43__
            len2Bits = (len2Bits << 24 | len2Bits) & 0xF0FFFF000000; // byte order is now 0_4321______
            return (long)(((len2Bits >> 20) & 0xFFFF0) | (len2Bits >> 44)); // byte order is now 43210
        }
    }
}
