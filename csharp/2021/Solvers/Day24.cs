using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day24 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        Span<(int CharIndex, int Offset)> offsets = stackalloc (int CharIndex, int Offset)[8];
        int offsetIndex = 1;

        Span<char> part1 = stackalloc char[14];
        part1.Fill('9');

        Span<char> part2 = stackalloc char[14];
        part2.Fill('1');

        int inputCursor = 0;
        for (int charIndex = 0; charIndex < 14; charIndex++)
        {
            ParseCodeSection(input, ref inputCursor, out bool _, out int offsetForPop, out int offsetForPush);

            (int lastCharIndex, int lastOffset) = offsets[offsetIndex - 1];
            int diff = lastOffset + offsetForPop;

            // Must push since impossible to get a diff of 9 or higher
            if (diff >= 9)
            {
                offsets[offsetIndex++] = (charIndex, offsetForPush);
            }
            else if (diff > 0)
            {
                part1[lastCharIndex] = (char)('9' - diff);
                part2[charIndex] = (char)('1' + diff);
                offsetIndex--;
            }
            else
            {
                part1[charIndex] = (char)('9' + diff);
                part2[lastCharIndex] = (char)('1' - diff);
                offsetIndex--;
            }
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    private static void ParseCodeSection(ReadOnlySpan<byte> input, ref int i, out bool canPop, out int offsetForPop, out int offsetForPush)
    {
        i += "inp w\nmul x 0\nadd x z\nmod x 26\ndiv z ".Length;
        canPop = input[i] == '2';
        i += (canPop ? "26".Length : "1".Length) + "\nadd x ".Length;
        offsetForPop = ReadIntegerFromInput(input, ref i);
        i += "eql x w\neql x 0\nmul y 0\nadd y 25\nmul y x\nadd y 1\nmul z y\nmul y 0\nadd y w\nadd y ".Length;
        offsetForPush = ReadIntegerFromInput(input, ref i);
        i += "mul y x\nadd z y\n".Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadIntegerFromInput(ReadOnlySpan<byte> span, ref int i)
    {
        // Assume that the first character is always a digit
        byte c = span[i++];

        int mul;
        int ret;
        if (c == '-')
        {
            mul = -1;
            ret = 0;
        }
        else
        {
            mul = 1;
            ret = c - '0';
        }

        byte cur;
        while ((cur = span[i++]) != '\n')
            ret = ret * 10 + (cur - '0');

        return mul * ret;
    }
}
