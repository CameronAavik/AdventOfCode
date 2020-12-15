using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

// using a type alias because some methods get too long otherwise
using Vec256 = System.Runtime.Intrinsics.Vector256<uint>;

namespace AdventOfCode.CSharp.Common
{
    public static class SIMDMD5
    {
        private static readonly Vec256 s_mask = Vector256.Create(uint.MaxValue);
        private static readonly Vec256 s_initA = Vector256.Create(0x67452301u);
        private static readonly Vec256 s_initB = Vector256.Create(0xefcdab89u);
        private static readonly Vec256 s_initC = Vector256.Create(0x98badcfeu);
        private static readonly Vec256 s_initD = Vector256.Create(0x10325476u);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vec256 F(Vec256 x, Vec256 y, Vec256 z) => Avx2.Or(Avx2.And(x, y), Avx2.AndNot(x, z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vec256 G(Vec256 x, Vec256 y, Vec256 z) => Avx2.Or(Avx2.And(x, z), Avx2.AndNot(z, y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vec256 H(Vec256 x, Vec256 y, Vec256 z) => Avx2.Xor(x, Avx2.Xor(y, z));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vec256 I(Vec256 x, Vec256 y, Vec256 z) => Avx2.Xor(y, Avx2.Or(x, Avx2.AndNot(z, s_mask)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vec256 RotateLeft(Vec256 a, byte s) =>
            Avx2.Xor(Avx2.ShiftLeftLogical(a, s), Avx2.ShiftRightLogical(a, (byte)(32 - s)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void FF(ref Vec256 a, Vec256 b, Vec256 c, Vec256 d, Vec256 x, byte s, uint t)
        {
            a = Avx2.Add(a, Vector256.Create(t));
            a = Avx2.Add(a, x);
            a = Avx2.Add(a, F(b, c, d));
            a = RotateLeft(a, s);
            a = Avx2.Add(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GG(ref Vec256 a, Vec256 b, Vec256 c, Vec256 d, Vec256 x, byte s, uint t)
        {
            a = Avx2.Add(a, Vector256.Create(t));
            a = Avx2.Add(a, x);
            a = Avx2.Add(a, G(b, c, d));
            a = RotateLeft(a, s);
            a = Avx2.Add(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void HH(ref Vec256 a, Vec256 b, Vec256 c, Vec256 d, Vec256 x, byte s, uint t)
        {
            a = Avx2.Add(a, Vector256.Create(t));
            a = Avx2.Add(a, x);
            a = Avx2.Add(a, H(b, c, d));
            a = RotateLeft(a, s);
            a = Avx2.Add(a, b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void II(ref Vec256 a, Vec256 b, Vec256 c, Vec256 d, Vec256 x, byte s, uint t)
        {
            a = Avx2.Add(a, Vector256.Create(t));
            a = Avx2.Add(a, x);
            a = Avx2.Add(a, I(b, c, d));
            a = RotateLeft(a, s);
            a = Avx2.Add(a, b);
        }

        /// <summary>
        /// Performs MD5, but only returns the first 8 bytes since that is what I want
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Vec256 TransformAndReturnFirst4Bytes(Vec256[] data)
        {
            Vec256 a = s_initA, b = s_initB, c = s_initC, d = s_initD;

            /* Round 1 */
            FF(ref a, b, c, d, data[0], 7, 0xd76aa478); /* 1 */
            FF(ref d, a, b, c, data[1], 12, 0xe8c7b756); /* 2 */
            FF(ref c, d, a, b, data[2], 17, 0x242070db); /* 3 */
            FF(ref b, c, d, a, data[3], 22, 0xc1bdceee); /* 4 */
            FF(ref a, b, c, d, data[4], 7, 0xf57c0faf); /* 5 */
            FF(ref d, a, b, c, data[5], 12, 0x4787c62a); /* 6 */
            FF(ref c, d, a, b, data[6], 17, 0xa8304613); /* 7 */
            FF(ref b, c, d, a, data[7], 22, 0xfd469501); /* 8 */
            FF(ref a, b, c, d, data[8], 7, 0x698098d8); /* 9 */
            FF(ref d, a, b, c, data[9], 12, 0x8b44f7af); /* 10 */
            FF(ref c, d, a, b, data[10], 17, 0xffff5bb1); /* 11 */
            FF(ref b, c, d, a, data[11], 22, 0x895cd7be); /* 12 */
            FF(ref a, b, c, d, data[12], 7, 0x6b901122); /* 13 */
            FF(ref d, a, b, c, data[13], 12, 0xfd987193); /* 14 */
            FF(ref c, d, a, b, data[14], 17, 0xa679438e); /* 15 */
            FF(ref b, c, d, a, data[15], 22, 0x49b40821); /* 16 */

            /* Round 2 */
            GG(ref a, b, c, d, data[1], 5, 0xf61e2562); /* 17 */
            GG(ref d, a, b, c, data[6], 9, 0xc040b340); /* 18 */
            GG(ref c, d, a, b, data[11], 14, 0x265e5a51); /* 19 */
            GG(ref b, c, d, a, data[0], 20, 0xe9b6c7aa); /* 20 */
            GG(ref a, b, c, d, data[5], 5, 0xd62f105d); /* 21 */
            GG(ref d, a, b, c, data[10], 9, 0x2441453); /* 22 */
            GG(ref c, d, a, b, data[15], 14, 0xd8a1e681); /* 23 */
            GG(ref b, c, d, a, data[4], 20, 0xe7d3fbc8); /* 24 */
            GG(ref a, b, c, d, data[9], 5, 0x21e1cde6); /* 25 */
            GG(ref d, a, b, c, data[14], 9, 0xc33707d6); /* 26 */
            GG(ref c, d, a, b, data[3], 14, 0xf4d50d87); /* 27 */
            GG(ref b, c, d, a, data[8], 20, 0x455a14ed); /* 28 */
            GG(ref a, b, c, d, data[13], 5, 0xa9e3e905); /* 29 */
            GG(ref d, a, b, c, data[2], 9, 0xfcefa3f8); /* 30 */
            GG(ref c, d, a, b, data[7], 14, 0x676f02d9); /* 31 */
            GG(ref b, c, d, a, data[12], 20, 0x8d2a4c8a); /* 32 */

            /* Round 3 */
            HH(ref a, b, c, d, data[5], 4, 0xfffa3942); /* 33 */
            HH(ref d, a, b, c, data[8], 11, 0x8771f681); /* 34 */
            HH(ref c, d, a, b, data[11], 16, 0x6d9d6122); /* 35 */
            HH(ref b, c, d, a, data[14], 23, 0xfde5380c); /* 36 */
            HH(ref a, b, c, d, data[1], 4, 0xa4beea44); /* 37 */
            HH(ref d, a, b, c, data[4], 11, 0x4bdecfa9); /* 38 */
            HH(ref c, d, a, b, data[7], 16, 0xf6bb4b60); /* 39 */
            HH(ref b, c, d, a, data[10], 23, 0xbebfbc70); /* 40 */
            HH(ref a, b, c, d, data[13], 4, 0x289b7ec6); /* 41 */
            HH(ref d, a, b, c, data[0], 11, 0xeaa127fa); /* 42 */
            HH(ref c, d, a, b, data[3], 16, 0xd4ef3085); /* 43 */
            HH(ref b, c, d, a, data[6], 23, 0x4881d05); /* 44 */
            HH(ref a, b, c, d, data[9], 4, 0xd9d4d039); /* 45 */
            HH(ref d, a, b, c, data[12], 11, 0xe6db99e5); /* 46 */
            HH(ref c, d, a, b, data[15], 16, 0x1fa27cf8); /* 47 */
            HH(ref b, c, d, a, data[2], 23, 0xc4ac5665); /* 48 */

            /* Round 4 */
            II(ref a, b, c, d, data[0], 6, 0xf4292244); /* 49 */
            II(ref d, a, b, c, data[7], 10, 0x432aff97); /* 50 */
            II(ref c, d, a, b, data[14], 15, 0xab9423a7); /* 51 */
            II(ref b, c, d, a, data[5], 21, 0xfc93a039); /* 52 */
            II(ref a, b, c, d, data[12], 6, 0x655b59c3); /* 53 */
            II(ref d, a, b, c, data[3], 10, 0x8f0ccc92); /* 54 */
            II(ref c, d, a, b, data[10], 15, 0xffeff47d); /* 55 */
            II(ref b, c, d, a, data[1], 21, 0x85845dd1); /* 56 */
            II(ref a, b, c, d, data[8], 6, 0x6fa87e4f); /* 57 */
            II(ref d, a, b, c, data[15], 10, 0xfe2ce6e0); /* 58 */
            II(ref c, d, a, b, data[6], 15, 0xa3014314); /* 59 */
            II(ref b, c, d, a, data[13], 21, 0x4e0811a1); /* 60 */
            II(ref a, b, c, d, data[4], 6, 0xf7537e82); /* 61 */
            // don't need to do last three steps if only looking at first 8 bytes
            //II(ref d, a, b, c, data[11], 10, 0xbd3af235); /* 62 */
            //II(ref c, d, a, b, data[2], 15, 0x2ad7d2bb); /* 63 */
            //II(ref b, c, d, a, data[9], 21, 0xeb86d391); /* 64 */

            return Avx2.Add(a, s_initA);
        }
    }
}
