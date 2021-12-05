using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day08 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        bool[,] pixels = new bool[50, 6];

        bool[] rowBuffer = new bool[50];
        bool[] colBuffer = new bool[6];
        foreach (ReadOnlySpan<char> line in input.SplitLines())
        {
            if (line[1] == 'e') // rect
            {
                ReadOnlySpan<char> dimensions = line.Slice(5);
                int xIndex = dimensions.IndexOf('x');
                int width = int.Parse(dimensions.Slice(0, xIndex));
                int height = int.Parse(dimensions.Slice(xIndex + 1));
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
                ReadOnlySpan<char> rotateData = line.Slice(16);
                int colLength = rotateData.IndexOf(' ');
                int column = int.Parse(rotateData.Slice(0, colLength));
                int rotateAmount = int.Parse(rotateData.Slice(colLength + 4));

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
                ReadOnlySpan<char> rotateData = line.Slice(13);
                int row = rotateData[0] - '0';
                int rotateAmount = int.Parse(rotateData.Slice(5));

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

            part2[i] = OCR(letterPixels);
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    // borrowing OCR table from https://github.com/willkill07/AdventOfCode2016/blob/master/src/Day08.cpp
    public static char OCR(int letterPixels) => letterPixels switch
    {
        0x19297A52 => 'A',
        0x392E4A5C => 'B',
        0x1928424C => 'C',
        0x39294A5C => 'D',
        0x3D0E421E => 'E',
        0x3D0E4210 => 'F',
        0x19285A4E => 'G',
        0x252F4A52 => 'H',
        0x1C42108E => 'I',
        0x0C210A4C => 'J',
        0x254C5292 => 'K',
        0x2108421E => 'L',
        0x23BAC631 => 'M',
        0x252D5A52 => 'N',
        0x19294A4C => 'O',
        0x39297210 => 'P',
        0x19295A4D => 'Q',
        0x39297292 => 'R',
        0x1D08305C => 'S',
        0x1C421084 => 'T',
        0x25294A4C => 'U',
        0x2318A944 => 'V',
        0x231AD6AA => 'W',
        0x22A22951 => 'X',
        0x23151084 => 'Y',
        0x3C22221E => 'Z',
        0x00000000 => ' ',
        _ => '?'
    };
}
