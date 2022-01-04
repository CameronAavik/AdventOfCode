using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day08 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        bool[,] pixels = new bool[50, 6];

        bool[] rowBuffer = new bool[50];
        bool[] colBuffer = new bool[6];
        foreach (ReadOnlySpan<byte> line in input.SplitLines())
        {
            if (line[1] == 'e') // rect
            {
                var reader = new SpanReader(line.Slice("rect ".Length));
                int width = reader.ReadPosIntUntil('x');
                int height = reader.ReadPosIntUntilEnd();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        pixels[x, y] = true;
                    }
                }
            }
            else if (line[7] == 'c') // rotate column
            {
                var reader = new SpanReader(line.Slice("rotate column x=".Length));
                int column = reader.ReadPosIntUntil(' ');
                reader.SkipLength("by ".Length);
                int rotateAmount = reader.ReadPosIntUntilEnd();

                for (int i = 0; i < 6; i++)
                {
                    colBuffer[i] = pixels[column, i];
                    int target = i - rotateAmount;
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
                var reader = new SpanReader(line.Slice("rotate row y=".Length));
                int row = reader.Peek() - '0';
                reader.SkipLength("0 by ".Length);
                int rotateAmount = reader.ReadPosIntUntilEnd();

                for (int i = 0; i < 50; i++)
                {
                    rowBuffer[i] = pixels[i, row];
                    int target = i - rotateAmount;
                    if (target < 0)
                    {
                        target += 50;
                    }

                    // if the target has already been swapped, get it from the buffer
                    pixels[i, row] = target < i ? rowBuffer[target] : pixels[target, row];
                }
            }
        }

        int part1 = 0;
        foreach (bool pixel in pixels)
        {
            if (pixel)
            {
                part1++;
            }
        }

        Span<char> part2 = stackalloc char[10];
        for (int i = 0; i < 10; i++)
        {
            int letterPixels = 0;
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 5; col++)
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
