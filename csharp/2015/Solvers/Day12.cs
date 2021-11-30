using AdventOfCode.CSharp.Common;
using System;
using System.Linq;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day12 : ISolver
{
    public Solution Solve(ReadOnlySpan<char> input)
    {
        int part1 = SolvePart1(input);
        int part2 = SolvePart2(input);

        return new Solution(part1, part2);
    }

    private static int SolvePart1(ReadOnlySpan<char> input)
    {
        int part1 = 0;

        int curNum = 0;
        int sign = 1;
        bool inNumber = false;
        char prev = '\0';
        foreach (char c in input)
        {
            int digit = c - '0';
            if (inNumber)
            {
                if (digit is >= 0 and < 10)
                {
                    curNum = curNum * 10 + digit;
                }
                else
                {
                    part1 += sign * curNum;
                    inNumber = false;
                }
            }
            else if (digit is >= 0 and < 10)
            {
                inNumber = true;
                curNum = digit;
                sign = prev == '-' ? -1 : 1;
            }

            prev = c;
        }

        return part1;
    }

    private static int SolvePart2(ReadOnlySpan<char> input)
    {
        int i = 0;
        return ParseValue(input, ref i, out _);
    }

    private static int ParseValue(ReadOnlySpan<char> input, ref int i, out bool isRedString)
    {
        isRedString = false;
        return input[i] switch
        {
            '{' => ParseObject(input, ref i),
            '[' => ParseArray(input, ref i),
            '\"' => ParseString(input, ref i, out isRedString),
            >= '0' and <= '9' => ParseNumber(input, isNegative: false, ref i),
            '-' => ParseNumber(input, isNegative: true, ref i),
            _ => 0,
        };
    }

    private static int ParseObject(ReadOnlySpan<char> input, ref int i)
    {
        // check if it is an empty object first
        if (input[i + 1] == '}')
        {
            i += 2;
            return 0;
        }

        bool isRedString = false;
        int total = 0;
        while (input[i++] != '}')
        {
            // parse the property name
            _ = ParseString(input, ref i, out _);

            // skip the colon
            i += 1;

            // parse the value
            total += ParseValue(input, ref i, out bool isValueRedString);
            isRedString = isRedString || isValueRedString;
        }

        return isRedString ? 0 : total;
    }

    private static int ParseArray(ReadOnlySpan<char> input, ref int i)
    {
        // check if it is an empty array first
        if (input[i + 1] == ']')
        {
            i += 2;
            return 0;
        }

        int total = 0;
        while (input[i++] != ']')
        {
            total += ParseValue(input, ref i, out _);
        }
        return total;
    }

    private static int ParseNumber(ReadOnlySpan<char> input, bool isNegative, ref int i)
    {
        if (isNegative)
        {
            i++;
        }

        int number = 0;
        char c;
        while ((c = input[i]) is >= '0' and <= '9')
        {
            number = 10 * number + (c - '0');
            i++;
        }

        return (isNegative ? -1 : 1) * number;
    }

    private static int ParseString(ReadOnlySpan<char> input, ref int i, out bool isRedString)
    {
        i++;
        int stringEnd = input[i..].IndexOf('\"');
        isRedString = stringEnd == 3 && input.Slice(i, 3).SequenceEqual("red");
        i += stringEnd + 1;
        return 0;
    }
}
