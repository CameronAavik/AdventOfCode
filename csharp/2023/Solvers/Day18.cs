using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day18 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        ref byte cur = ref MemoryMarshal.GetReference(input);
        ref byte end = ref Unsafe.Add(ref cur, (nuint)input.Length);

        int y1 = 0;
        long y2 = 0;
        int perimeter1 = 0;
        int area1 = 0;
        long perimeter2 = 0;
        long area2 = 0;

        while (Unsafe.IsAddressLessThan(ref cur, ref end))
        {
            byte dir1 = cur;

            int offset = Unsafe.Add(ref cur, 3) == ' ' ? 0 : 1; // gets compiled to branchless version

            cur = ref Unsafe.Add(ref cur, (nuint)(offset + 1));
            int len1Bits = Unsafe.As<byte, ushort>(ref cur) & 0xF0F;
            int len1 = ((len1Bits >> 8) + len1Bits * 10) & 0xFF; // reads the two bytes that store the first length and converts it to an integer

            switch (dir1)
            {
                case (byte)'R': area1 += len1 * y1; break;
                case (byte)'D': y1 += len1; break;
                case (byte)'L': area1 -= len1 * y1; break;
                case (byte)'U': y1 -= len1; break;
            }

            perimeter1 += len1;

            cur = ref Unsafe.Add(ref cur, 5);

            ulong hexBits = Unsafe.As<byte, ulong>(ref cur); // read 8 bytes into a 64 bit integer starting at hex
            int dir2 = (int)((hexBits >> 40) & 0xFF);

            ulong len2Bits = (hexBits & 0x0F0F0F0F0FUL) + 9 * ((hexBits >> 6) & 0x0101010101UL); // byte order is _0_1_2_3_4
            len2Bits = (len2Bits << 12 | len2Bits) & 0xF000FF00FF00; // byte order is now 0___21__43__
            len2Bits = (len2Bits << 24 | len2Bits) & 0xF0FFFF000000; // byte order is now 0_4321______
            long len2 = (long)(((len2Bits >> 20) & 0xFFFF0) | (len2Bits >> 44)); // byte order is now 43210

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

            cur = ref Unsafe.Add(ref cur, 8);
        }

        // Use Pick's theorem to calculate the total area
        int part1 = Math.Abs(area1) + perimeter1 / 2 + 1;
        long part2 = Math.Abs(area2) + perimeter2 / 2 + 1;

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
