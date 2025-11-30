using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2024.Solvers;

public class Day03 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var part1 = 0;
        var part2 = 0;

        const uint mulBytes = 0x286C756DU; // "mul(" as a little endian uint
        const uint doBytes = 0x29286F64U; // "do()" as a little endian uint
        const ulong dontBytes = 0x292874276E6F64UL; // "don't()" as a little endian ulong
        const ulong dontMask = 0xFFFFFFFFFFFFFFUL; // mask to check for "don't()" in a ulong

        var isEnabled = true;
        while (input.Length != 0)
        {
            var candidateIndex = input.IndexOfAny((byte)'d', (byte)'m');
            if (candidateIndex < 0)
                break;

            input = input[candidateIndex..];

            if (input[0] == 'm') // check for mul(a,b)
            {
                if (input.Length >= 8) // "mul(a,b)" is 8 characters long
                {
                    var mulCandidate = BinaryPrimitives.ReadUInt32LittleEndian(input);
                    if (mulCandidate == mulBytes)
                    {
                        var i = 4;
                        var a = ParseNumber(input, ref i, out var separator);
                        if (separator == (byte)',')
                        {
                            var b = ParseNumber(input, ref i, out separator);
                            if (separator == (byte)')')
                            {
                                var mul = a * b;
                                part1 += mul;
                                if (isEnabled)
                                    part2 += mul;

                                input = input[i..];
                                continue;
                            }
                        }

                        input = input[(i - 1)..]; // subtract 1 because the separator might be the start of another instruction
                        continue;
                    }
                }
            }
            else if (isEnabled) // check for don't()
            {
                if (input.Length >= 8) // even though "don't()" is 7 digits long, file always ends in newline so can safely load 8 bytes
                {
                    var dontCandidate = BinaryPrimitives.ReadUInt64LittleEndian(input);
                    if ((dontCandidate & dontMask) == dontBytes)
                    {
                        isEnabled = false;
                        input = input[7..];
                        continue;
                    }
                }
            }
            else // check for do()
            {
                if (input.Length >= 4) // "do()" is 4 digits long
                {
                    var doCandidate = BinaryPrimitives.ReadUInt32LittleEndian(input);
                    if (doCandidate == doBytes)
                    {
                        isEnabled = true;
                        input = input[4..];
                        continue;
                    }
                }
            }

            input = input[1..];
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ParseNumber(ReadOnlySpan<byte> input, ref int i, out byte c)
    {
        c = 0;
        var a = 0;
        while (i < input.Length && (c = input[i++]) is >= (byte)'0' and <= (byte)'9')
            a = a * 10 + c - '0';
        return a;
    }
}
