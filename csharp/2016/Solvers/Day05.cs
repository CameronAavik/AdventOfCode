using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers
{
    public class Day05 : ISolver
    {
        private static readonly Vector256<uint>[] s_asciiNumMasks = new Vector256<uint>[]
        {
            Vector256.Create(0x80u),
            Vector256.Create(0x80u << 8 | '0'),
            Vector256.Create(0x80u << 16 | '0' << 8 | '0'),
            Vector256.Create(0x80u << 24 | '0' << 16 | '0' << 8 | '0'),
            Vector256.Create((uint)'0' << 24 | '0' << 16 | '0' << 8 | '0'),
        };

        private class PasswordBuilder
        {
            private static readonly Vector256<uint> s_zeroMask = Vector256.Create(0xF0FFFFu);

            private readonly char[] _part1 = new char[8];
            private readonly char[] _part2 = new char[8];

            private int _part1Index = 0;
            private int _part2Seen = 0;

            public bool IsComplete { get; private set; } = false;

            public string Part1 => new(_part1);

            public string Part2 => new(_part2);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddHashes(Vector256<uint> hashVector, int i, int maxI)
            {
                Vector256<uint> masked = Avx2.And(s_zeroMask, hashVector);
                Vector256<uint> isZero = Avx2.CompareEqual(masked, Vector256<uint>.Zero);
                int moveMask = Avx2.MoveMask(isZero.AsByte());

                if (moveMask != 0)
                {
                    for (int j = 0; j < Math.Min(8, maxI - i); j++)
                    {
                        if ((moveMask & 0xF) != 0)
                        {
                            uint matchingHash = hashVector.GetElement(j);
                            int digit6 = (int)(matchingHash >> 16) & 0xF;
                            int digit7 = (int)(matchingHash >> 28) & 0xF;

                            if (_part1Index < 8)
                            {
                                _part1[_part1Index++] = IntToHex(digit6);
                            }

                            if (digit6 < 8 && _part2[digit6] == 0)
                            {
                                _part2[digit6] = IntToHex(digit7);
                                if (++_part2Seen == 8)
                                {
                                    IsComplete = true;
                                }
                            }
                        }

                        moveMask >>= 4;
                    }
                }
            }

            private static char IntToHex(int n) => (char)(n < 10 ? (n + '0') : (n - 10 + 'a'));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint NumToDigitsAsBytes(int x, int numDigits)
        {
            switch (numDigits)
            {
                case 1: return (uint)x;
                case 2:
                    int tens = Math.DivRem(x, 10, out int ones);
                    return (uint)(ones << 8 | tens);
                case 3:
                    int hundreds = Math.DivRem(x, 100, out tens);
                    tens = Math.DivRem(tens, 10, out ones);
                    return (uint)(ones << 16 | tens << 8 | hundreds);
                default:
                    int thousands = Math.DivRem(x, 1000, out hundreds);
                    hundreds = Math.DivRem(hundreds, 100, out tens);
                    tens = Math.DivRem(tens, 10, out ones);
                    return (uint)(ones << 24 | tens << 16 | hundreds << 8 | thousands);
            }
        }

        public Solution Solve(ReadOnlySpan<char> input)
        {
            // data contains the password being hashed in a format optimised for the vectorised MD5 implementation
            Vector256<uint>[] data = new Vector256<uint>[16];
            data[0] = Vector256.Create((uint)input[3] << 24 | (uint)input[2] << 16 | (uint)input[1] << 8 | input[0]);
            data[1] = Vector256.Create((uint)input[7] << 24 | (uint)input[6] << 16 | (uint)input[5] << 8 | input[4]);

            var pw = new PasswordBuilder();

            int start = 1;
            for (int len = 1; len <= 4; len++)
            {
                // need to put message end bit in 3rd uint when 2nd uint is full
                if (len == 4)
                {
                    data[3] = s_asciiNumMasks[0];
                }

                Vector256<uint> mask = s_asciiNumMasks[len];
                data[14] = Vector256.Create((uint)(len + 8) * 8);

                int end = start * 10;
                for (int i = start; i < end; i += 8)
                {
                    uint u1 = NumToDigitsAsBytes(i, len);
                    uint u2 = NumToDigitsAsBytes(i + 1, len);
                    uint u3 = NumToDigitsAsBytes(i + 2, len);
                    uint u4 = NumToDigitsAsBytes(i + 3, len);
                    uint u5 = NumToDigitsAsBytes(i + 4, len);
                    uint u6 = NumToDigitsAsBytes(i + 5, len);
                    uint u7 = NumToDigitsAsBytes(i + 6, len);
                    uint u8 = NumToDigitsAsBytes(i + 7, len);
                    var v = Vector256.Create(u1, u2, u3, u4, u5, u6, u7, u8);

                    data[2] = Avx2.Or(mask, v);
                    pw.AddHashes(SIMDMD5.TransformAndReturnFirst4Bytes(data), i, end);
                }

                start = end;
            }

            int upperDiv = 10;
            for (int len = 5; len <= 8; len++)
            {
                // need to put message end bit in 4rd uint when 3nd uint is full
                if (len == 8)
                {
                    data[4] = s_asciiNumMasks[0];
                }

                Vector256<uint> mask1 = s_asciiNumMasks[4];
                Vector256<uint> mask2 = s_asciiNumMasks[len - 4];
                data[14] = Vector256.Create((uint)(len + 8) * 8);

                int end = start * 10;
                for (int i = start; i < end; i += 8)
                {
                    Vector256<uint> v1;
                    if (i / upperDiv == (i + 7) / upperDiv)
                    {
                        uint u = NumToDigitsAsBytes(i / upperDiv, 4);
                        v1 = Vector256.Create(u);
                    }
                    else
                    {
                        uint u1 = NumToDigitsAsBytes(i / upperDiv, 4);
                        uint u2 = NumToDigitsAsBytes((i + 1) / upperDiv, 4);
                        uint u3 = NumToDigitsAsBytes((i + 2) / upperDiv, 4);
                        uint u4 = NumToDigitsAsBytes((i + 3) / upperDiv, 4);
                        uint u5 = NumToDigitsAsBytes((i + 4) / upperDiv, 4);
                        uint u6 = NumToDigitsAsBytes((i + 5) / upperDiv, 4);
                        uint u7 = NumToDigitsAsBytes((i + 6) / upperDiv, 4);
                        uint u8 = NumToDigitsAsBytes((i + 7) / upperDiv, 4);
                        v1 = Vector256.Create(u1, u2, u3, u4, u5, u6, u7, u8);
                    }

                    data[2] = Avx2.Or(mask1, v1);

                    uint u1_2 = NumToDigitsAsBytes(i % upperDiv, len - 4);
                    uint u2_2 = NumToDigitsAsBytes((i + 1) % upperDiv, len - 4);
                    uint u3_2 = NumToDigitsAsBytes((i + 2) % upperDiv, len - 4);
                    uint u4_2 = NumToDigitsAsBytes((i + 3) % upperDiv, len - 4);
                    uint u5_2 = NumToDigitsAsBytes((i + 4) % upperDiv, len - 4);
                    uint u6_2 = NumToDigitsAsBytes((i + 5) % upperDiv, len - 4);
                    uint u7_2 = NumToDigitsAsBytes((i + 6) % upperDiv, len - 4);
                    uint u8_2 = NumToDigitsAsBytes((i + 7) % upperDiv, len - 4);
                    var v2 = Vector256.Create(u1_2, u2_2, u3_2, u4_2, u5_2, u6_2, u7_2, u8_2);

                    data[3] = Avx2.Or(mask2, v2);
                    pw.AddHashes(SIMDMD5.TransformAndReturnFirst4Bytes(data), i, end);

                    if (pw.IsComplete)
                    {
                        break;
                    }
                }

                if (pw.IsComplete)
                {
                    break;
                }

                start = end;
                upperDiv *= 10;
            }

            return new Solution(pw.Part1, pw.Part2);
        }
    }
}
