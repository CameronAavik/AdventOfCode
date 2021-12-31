using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day01 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        // The counters which will store the solutions to both parts
        int part1 = 0;
        int part2 = 0;

        // A value that represents the index in the input we are currently looking at
        int inputCursor = 0;

        // a, b, and c will store the three most recent measurements seen
        int a = ReadLineAsInteger(input, ref inputCursor);
        int b = ReadLineAsInteger(input, ref inputCursor);
        int c = ReadLineAsInteger(input, ref inputCursor);

        // Check if the value increased within the first three readings
        if (b > a) part1++;
        if (c > b) part1++;

        // Each iteration of the while processes 3 measurements
        // The first measurement goes into "a", the second into "b", and the third into "c"
        // For part 1 we always compare each measurement against the previous one
        // For part 2 we compare with the measurement from 3 readings ago
        // This works because (a + b + c) > (b + c + d) can be simplified to a > d 
        while (inputCursor < input.Length)
        {
            // Measurement 1
            int m = ReadLineAsInteger(input, ref inputCursor);
            if (m > c) part1++;
            if (m > a) part2++;
            a = m;

            if (inputCursor >= input.Length)
                break;

            // Measurement 2
            m = ReadLineAsInteger(input, ref inputCursor);
            if (m > a) part1++;
            if (m > b) part2++;
            b = m;

            if (inputCursor >= input.Length)
                break;

            // Measurement 3
            m = ReadLineAsInteger(input, ref inputCursor);
            if (m > b) part1++;
            if (m > c) part2++;
            c = m;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ReadLineAsInteger(ReadOnlySpan<char> span, ref int i)
    {
        // Assume that the first character is always a digit
        int ret = span[i++] - '0';

        char cur;
        while ((cur = span[i++]) != '\n')
            ret = ret * 10 + (cur - '0');

        return ret;
    }
}
