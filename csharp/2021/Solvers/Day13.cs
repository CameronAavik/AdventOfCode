using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day13 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        int foldsIndex = input.IndexOf((byte)'f');
        ReadOnlySpan<byte> foldsInput = input.Slice(foldsIndex);
        ReadOnlySpan<byte> dotsInput = input.Slice(0, foldsIndex - 1);

        Span<int> xFolds = stackalloc int[32];
        Span<int> yFolds = stackalloc int[32];
        ParseFolds(foldsInput, xFolds, yFolds, out int numXFolds, out int numYFolds, out bool firstFoldIsX);

        var firstFoldAxis = firstFoldIsX ? xFolds[0] : yFolds[0];

        int maxX = xFolds[0] * 2 + 1;
        int maxY = yFolds[0] * 2 + 1;

        Span<byte> finalXPositions = stackalloc byte[maxX + 1];
        Span<byte> finalYPositions = stackalloc byte[maxY + 1];

        for (byte x = 0; x < 40; x++)
            finalXPositions[x] = x;

        for (int i = numXFolds - 1; i >= 0; i--)
        {
            int fold = xFolds[i];
            var dst = finalXPositions.Slice(fold + 1, fold);
            finalXPositions.Slice(0, fold).CopyTo(dst);
            dst.Reverse();
        }

        for (byte y = 0; y < 6; y++)
            finalYPositions[y] = y;

        for (int i = numYFolds - 1; i >= 0; i--)
        {
            int fold = yFolds[i];
            var dst = finalYPositions.Slice(fold + 1, fold);
            finalYPositions.Slice(0, fold).CopyTo(dst);
            dst.Reverse();
        }

        Span<int> letterMasks = stackalloc int[8];
        var dotsAfterOneFold = new HashSet<int>();

        int dotsInputCursor = 0;
        while (dotsInputCursor < dotsInput.Length)
        {
            ParseDot(dotsInput, ref dotsInputCursor, out int x, out int y);

            if (firstFoldIsX)
            {
                if (x > firstFoldAxis)
                    x = 2 * firstFoldAxis - x;
            }
            else
            {
                if (y > firstFoldAxis)
                    y = 2 * firstFoldAxis - y;
            }

            if (dotsAfterOneFold.Add((x << 16) | y))
            {
                x = finalXPositions[x];
                y = finalYPositions[y];

                (int letter, int col) = Math.DivRem(x, 5);

                letterMasks[letter] |= 1 << (29 - (y * 5 + col));
            }
        }

        solution.SubmitPart1(dotsAfterOneFold.Count);

        Span<char> letters = stackalloc char[8];
        for (int i = 0; i < 8; i++)
            letters[i] = OCR.MaskToLetter(letterMasks[i]);

        solution.SubmitPart2(letters);
    }

    private static void ParseFolds(ReadOnlySpan<byte> foldsInput, Span<int> xfolds, Span<int> yFolds, out int numXFolds, out int numYFolds, out bool firstFoldIsX)
    {
        numXFolds = 0;
        numYFolds = 0;

        bool isFirstFold = true;
        firstFoldIsX = false;

        int foldInputCursor = 0;
        while (foldInputCursor < foldsInput.Length)
        {
            foldInputCursor += "fold along ".Length;
            bool isX = foldsInput[foldInputCursor] == 'x';
            foldInputCursor += "x=".Length;

            int axis = foldsInput[foldInputCursor++] - '0';
            byte c;
            while ((c = foldsInput[foldInputCursor++]) != '\n')
                axis = axis * 10 + (c - '0');

            if (isFirstFold)
            {
                firstFoldIsX = isX;
                isFirstFold = false;
            }

            if (isX)
                xfolds[numXFolds++] = axis;
            else
                yFolds[numYFolds++] = axis;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ParseDot(ReadOnlySpan<byte> dotsInput, ref int dotsInputCursor, out int x, out int y)
    {
        x = dotsInput[dotsInputCursor++] - '0';

        byte c;
        while ((c = dotsInput[dotsInputCursor++]) != ',')
            x = x * 10 + (c - '0');

        y = dotsInput[dotsInputCursor++] - '0';

        while ((c = dotsInput[dotsInputCursor++]) != '\n')
            y = y * 10 + (c - '0');
    }
}
