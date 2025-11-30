using AdventOfCode.CSharp.Common;
using System;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day10 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        Span<byte> stack = stackalloc byte[4096];
        var sp = 0;

        Span<long> scores = stackalloc long[1024];
        var numScores = 0;

        var totalSyntaxError = 0;
        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            switch (c & 0b111)
            {
                case 0b010: // '\n'
                    long score = 0;
                    while (sp > 0)
                    {
                        score *= 5;
                        score += stack[--sp] switch
                        {
                            (byte)'(' => 1,
                            (byte)'[' => 2,
                            (byte)'{' => 3,
                            (byte)'<' or _ => 4
                        };
                    }

                    scores[numScores++] = score;
                    break;
                case 0b011: // '[', '{'
                case 0b000: // '('
                case 0b100: // '<'
                    stack[sp++] = c;
                    break;
                default: // will match all the closing characters
                    var top = stack[--sp];
                    if (c - top is not (1 or 2)) // all closing characters are 1 or 2 away from the opening character
                    {
                        totalSyntaxError += c switch
                        {
                            (byte)')' => 3,
                            (byte)']' => 57,
                            (byte)'}' => 1197,
                            (byte)'>' or _ => 25137
                        };

                        // skip to next line
                        sp = 0;
                        while (input[++i] != '\n')
                            ;
                    }
                    break;
            }
        }

        solution.SubmitPart1(totalSyntaxError);
        solution.SubmitPart2(FindMedian(scores[..numScores]));
    }

    private static long FindMedian(Span<long> scores)
    {
        var medianIndex = scores.Length / 2;
        while (scores.Length > 1)
        {
            if (medianIndex == 0)
                return FindMin(scores);

            if (medianIndex == scores.Length - 1)
                return FindMax(scores);

            var pivot = scores[0];
            var l = 1;
            var r = scores.Length - 1;
            while (l <= r)
            {
                var score = scores[l];
                if (score <= pivot)
                {
                    l++;
                }
                else
                {
                    scores[l] = scores[r];
                    scores[r] = score;
                    r--;
                }
            }

            if (l <= medianIndex)
            {
                medianIndex -= l;
                scores = scores[l..];
            }
            else if (l == medianIndex + 1)
            {
                return pivot;
            }
            else
            {
                scores = scores.Slice(1, l);
            }
        }

        return scores[0];
    }

    private static long FindMax(Span<long> scores)
    {
        var max = long.MinValue;
        foreach (var score in scores)
            if (score > max)
                max = score;
        return max;
    }

    private static long FindMin(Span<long> scores)
    {
        var min = long.MaxValue;
        foreach (var score in scores)
            if (score < min)
                min = score;
        return min;
    }
}
