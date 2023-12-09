using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2023.Solvers;

public class Day09 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int part1 = 0;
        int part2 = 0;

        while (input.Length > 0)
        {
            int lineEndIndex = input.IndexOf((byte)'\n');
            ReadOnlySpan<byte> line = input.Slice(0, lineEndIndex + 1);
            ProcessLine(line, ref part1, ref part2);
            input = input.Slice(lineEndIndex + 1);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [SkipLocalsInit]
    private static void ProcessLine(ReadOnlySpan<byte> line, ref int part1, ref int part2)
    {
        // Assuming each line has 24 numbers or less
        Span<int> lineNumbers = stackalloc int[24];

        int n = LoadNumbersFromLine(line, lineNumbers);
        ref int lineNumbersRef = ref MemoryMarshal.GetReference(lineNumbers);
        Vector256<int> numberVec1 = Vector256.LoadUnsafe(ref lineNumbersRef);
        Vector256<int> numberVec2 = Vector256.LoadUnsafe(ref lineNumbersRef, 8);
        Vector256<int> numberVec3 = Vector256.LoadUnsafe(ref lineNumbersRef, 16);

        int vecSize = n;

        // handle case where data fits in 3 vectors
        while (vecSize > 16)
        {
            int firstElement = numberVec1.GetElement(0);
            part1 += numberVec3.GetElement(vecSize - 17);
            part2 += (n - vecSize) % 2 == 1 ? -firstElement : firstElement;
            Vector256<int> shifted1 = RotateLeftOneByte(numberVec1);
            Vector256<int> shifted2 = RotateLeftOneByte(numberVec2);
            Vector256<int> shifted3 = RotateLeftOneByte(numberVec3);

            numberVec1 = MoveLastByteAcross(shifted1, shifted2) - numberVec1;
            numberVec2 = MoveLastByteAcross(shifted2, shifted3) - numberVec2;
            numberVec3 = shifted3 - numberVec3;

            vecSize--;

            if (numberVec1 == Vector256<int>.Zero && numberVec2 == Vector256<int>.Zero && FirstNBytesAreZero(numberVec3, vecSize - 16))
                return;
        }

        // handle case where data fits in 2 vectors
        while (vecSize > 8)
        {
            int firstElement = numberVec1.GetElement(0);
            part1 += numberVec2.GetElement(vecSize - 9);
            part2 += (n - vecSize) % 2 == 1 ? -firstElement : firstElement;

            Vector256<int> shifted1 = RotateLeftOneByte(numberVec1);
            Vector256<int> shifted2 = RotateLeftOneByte(numberVec2);

            numberVec1 = MoveLastByteAcross(shifted1, shifted2) - numberVec1;
            numberVec2 = shifted2 - numberVec2;

            vecSize--;

            if (numberVec1 == Vector256<int>.Zero && FirstNBytesAreZero(numberVec2, vecSize - 8))
                return;
        }

        // handle case where data fits in 1 vector
        while (vecSize > 0)
        {
            int firstElement = numberVec1.GetElement(0);
            part1 += numberVec1.GetElement(vecSize - 1);
            part2 += (n - vecSize) % 2 == 1 ? -firstElement : firstElement;

            numberVec1 = RotateLeftOneByte(numberVec1) - numberVec1;

            vecSize--;

            if (FirstNBytesAreZero(numberVec2, vecSize))
                return;
        }
    }

    private static int LoadNumbersFromLine(ReadOnlySpan<byte> line, Span<int> lineNumbers)
    {
        int n = 0;
        int i = 0;
        while (i < line.Length)
        {
            int num;
            byte c = line[i++];
            if (c == '-')
            {
                num = '0' - line[i++];
                while ((c = line[i++]) > ' ')
                    num = num * 10 + '0' - c;
            }
            else
            {
                num = c - '0';
                while ((c = line[i++]) > ' ')
                    num = num * 10 + c - '0';
            }

            lineNumbers[n++] = num;
        }

        return n;
    }

    private static Vector256<int> RotateLeftOneByte(Vector256<int> vec) =>
        Avx2.PermuteVar8x32(vec, Vector256.Create(1, 2, 3, 4, 5, 6, 7, 0));

    private static Vector256<int> MoveLastByteAcross(Vector256<int> vec1, Vector256<int> vec2) =>
        Avx2.Blend(vec1, vec2, 0b1000_0000);

    private static bool FirstNBytesAreZero(Vector256<int> vec, int bytes)
    {
        uint zeroBitIndexes = Vector256.Equals(vec, Vector256<int>.Zero).ExtractMostSignificantBits();
        uint mask = (uint)((1 << bytes) - 1);
        return (zeroBitIndexes & mask) == mask;
    }
}
