using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2024.Solvers;

// I will come back to add comments for this, but for now I have this done
public class Day04 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        ref byte inputRef = ref MemoryMarshal.GetReference(input);

        int lineLength = input.IndexOf((byte)'\n');
        int lines = input.Length / (lineLength + 1);

        int part1 = 0;
        int part2 = 0;
        int col = 0;
        while (col < lineLength)
        {
            ulong X_S = 0, M_S = 0;
            ulong XM_SA_V = 0, XMA_SAM_V = 0;
            ulong XM_SA_DR = 0, XMA_SAM_DR = 0, MA_SA_DR = 0;
            ulong XM_SA_DL = 0, XMA_SAM_DL = 0, MA_SA_DL = 0;

            // need to load with padding of 3 bytes on each side
            uint finalMask = 0xFFFFFFFF;
            int offset = col;

            if (col != 0) {
                finalMask &= 0xFFFFFFF8; // exclude first three columns except for the first loop
                offset -= 3;
            }

            int nextCol = offset + 32;
            if (nextCol >= lineLength)
            {
                offset = lineLength - 32;
                nextCol = lineLength;
                finalMask = 0xFFFFFFFF << (col - offset);
            }
            else
            {
                finalMask &= 0x1FFFFFFF;
                nextCol -= 3;
            }

            const ulong ShiftLeftMask =  0xFFFFFFFF7FFFFFFF;
            const ulong ShiftRightMask = 0xFFFFFFFEFFFFFFFF;
            ulong finalMask64 = finalMask | ((ulong)finalMask << 32);

            for (int row = 0; row < lines; row++)
            {
                var bytes = Vector256.LoadUnsafe(ref inputRef, (nuint)(offset + row * (lineLength + 1)));
                ulong x = (ulong)Vector256.Equals(bytes, Vector256.Create((byte)'X')).ExtractMostSignificantBits();
                ulong m = (ulong)Vector256.Equals(bytes, Vector256.Create((byte)'M')).ExtractMostSignificantBits();
                ulong a = (ulong)Vector256.Equals(bytes, Vector256.Create((byte)'A')).ExtractMostSignificantBits();
                ulong s = (ulong)Vector256.Equals(bytes, Vector256.Create((byte)'S')).ExtractMostSignificantBits();

                ulong xmas_h = x & (m >> 1) & (a >> 2) & (s >> 3) & finalMask64;
                part1 += BitOperations.PopCount(xmas_h);

                ulong samx_h = s & (a >> 1) & (m >> 2) & (x >> 3) & finalMask64;
                part1 += BitOperations.PopCount(samx_h);

                ulong s_x = (s | (x << 32)) & finalMask64;

                ulong xmas_samx_v = XMA_SAM_V & s_x;
                part1 += BitOperations.PopCount(xmas_samx_v);

                ulong xmas_samx_dr = ((XMA_SAM_DR & ShiftLeftMask) << 1) & s_x;
                part1 += BitOperations.PopCount(xmas_samx_dr);

                ulong xmas_samx_dl = ((XMA_SAM_DL & ShiftRightMask) >> 1) & s_x;
                part1 += BitOperations.PopCount(xmas_samx_dl);

                ulong s_m = s | (m << 32);
                ulong mas_sam_dr = (((MA_SA_DR & ShiftLeftMask) << 1) & s_m & ShiftRightMask) >> 1;
                ulong mas_sam_dl = (((MA_SA_DL & ShiftRightMask) >> 1) & s_m & ShiftLeftMask) << 1;

                mas_sam_dr |= mas_sam_dr >> 32;
                mas_sam_dl |= mas_sam_dl >> 32;                

                part2 += BitOperations.PopCount(mas_sam_dr & mas_sam_dl & finalMask);

                ulong a_m = a | (m << 32);
                XMA_SAM_V = XM_SA_V & a_m;
                XMA_SAM_DR = ((XM_SA_DR & ShiftLeftMask) << 1) & a_m;
                XMA_SAM_DL = ((XM_SA_DL & ShiftRightMask) >> 1) & a_m;

                ulong m_a = m | (a << 32);
                XM_SA_V = X_S & m_a;
                XM_SA_DR = ((X_S & ShiftLeftMask) << 1) & m_a;
                XM_SA_DL = ((X_S & ShiftRightMask) >> 1) & m_a;

                X_S = x | (s << 32);

                ulong a_a = a | (a << 32);
                MA_SA_DR = ((M_S & ShiftLeftMask) << 1) & a_a;
                MA_SA_DL = ((M_S & ShiftRightMask) >> 1) & a_a;

                M_S = m | (s << 32); 
            }

            col = nextCol;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
