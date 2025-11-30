using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day08 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var pixels = new bool[50, 6];

        var rowBuffer = new bool[50];
        var colBuffer = new bool[6];
        foreach (var lineRange in input.SplitLines())
        {
            var line = input[lineRange];
            if (line[1] == 'e') // rect
            {
                var reader = new SpanReader(line["rect ".Length..]);
                var width = reader.ReadPosIntUntil('x');
                var height = reader.ReadPosIntUntilEnd();
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        pixels[x, y] = true;
                    }
                }
            }
            else if (line[7] == 'c') // rotate column
            {
                var reader = new SpanReader(line["rotate column x=".Length..]);
                var column = reader.ReadPosIntUntil(' ');
                reader.SkipLength("by ".Length);
                var rotateAmount = reader.ReadPosIntUntilEnd();

                for (var i = 0; i < 6; i++)
                {
                    colBuffer[i] = pixels[column, i];
                    var target = i - rotateAmount;
                    if (target < 0)
                    {
                        target += 6;
                    }

                    // if the target has already been swapped, get it from the buffer
                    pixels[column, i] = target < i ? colBuffer[target] : pixels[column, target];
                }
            }
            else // rotate row
            {
                var reader = new SpanReader(line["rotate row y=".Length..]);
                var row = reader.Peek() - '0';
                reader.SkipLength("0 by ".Length);
                var rotateAmount = reader.ReadPosIntUntilEnd();

                for (var i = 0; i < 50; i++)
                {
                    rowBuffer[i] = pixels[i, row];
                    var target = i - rotateAmount;
                    if (target < 0)
                    {
                        target += 50;
                    }

                    // if the target has already been swapped, get it from the buffer
                    pixels[i, row] = target < i ? rowBuffer[target] : pixels[target, row];
                }
            }
        }

        var part1 = 0;
        foreach (var pixel in pixels)
        {
            if (pixel)
            {
                part1++;
            }
        }

        Span<char> part2 = stackalloc char[10];
        for (var i = 0; i < 10; i++)
        {
            var letterPixels = 0;
            for (var row = 0; row < 6; row++)
            {
                for (var col = 0; col < 5; col++)
                {
                    if (pixels[i * 5 + col, row])
                    {
                        letterPixels |= 1 << (29 - (row * 5 + col));
                    }
                }
            }

            part2[i] = OCR.MaskToLetter(letterPixels);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
