using AdventOfCode.CSharp.Common;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace AdventOfCode.CSharp.Y2021.Solvers;

public class Day01 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        // The counters which will store the solutions to both parts
        var part1 = 0;
        var part2 = 0;

        // A value that represents the index in the input we are currently looking at
        nint inputCursor = 0;

        // a, b, and c will store the three most recent measurements seen
        var a = ReadLineAsInteger(input, ref inputCursor);
        var b = ReadLineAsInteger(input, ref inputCursor);
        var c = ReadLineAsInteger(input, ref inputCursor);

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
            var m = ReadLineAsInteger(input, ref inputCursor);
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
    private static uint ReadLineAsInteger(ReadOnlySpan<byte> input, ref nint i)
    {
        static uint CharToInt(byte c)
        {
            return unchecked(c - (uint)'0');
        }

        ref var inputRef = ref MemoryMarshal.GetReference(input);

        // Assume that the first character is always a digit
        var ret = CharToInt(Unsafe.Add(ref inputRef, i++));

        byte cur;
        while ((cur = Unsafe.Add(ref inputRef, i++)) != '\n')
            ret = ret * 10 + CharToInt(cur);

        return ret;
    }
}
