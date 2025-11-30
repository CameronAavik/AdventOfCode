using System;
using System.Security.Cryptography;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day04 : ISolver
{
    public static void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        const byte zeroByte = (byte)'0';
        const byte oneByte = (byte)'1';

        using var provider = MD5.Create();

        // create a buffer for the MD5 provider to store the result hash in
        Span<byte> resultBuffer = stackalloc byte[provider.HashSize / 8];

        var inputAsBytes = input.TrimEnd((byte)'\n');

        var inputLen = inputAsBytes.Length;

        var extraBytes = 1;
        var n = 1;

        var part1 = 0;
        var part2 = 0;

        // each iteration of this while loop increases the number of digits being tested
        while (part2 == 0)
        {
            // buffer that stores the data being hashed
            var inputBuffer = new byte[inputLen + extraBytes];

            // populate the first bytes of the input buffer with the problem input
            inputAsBytes.CopyTo(inputBuffer);

            // put the ASCII representation of '1' as the first byte and '0' for the remaining bytes
            // this means that if we were testing 5 digits, then this would start at 10000
            inputBuffer[inputLen] = oneByte;
            for (var i = inputLen + 1; i < inputBuffer.Length; i++)
            {
                inputBuffer[i] = zeroByte;
            }

            // n will already be initialised at the correct value
            var nEnd = n * 10;
            for (; n < nEnd; n++)
            {
                _ = provider.TryComputeHash(inputBuffer, resultBuffer, out _);
                if (resultBuffer[0] == 0 && resultBuffer[1] == 0)
                {
                    if (part1 == 0)
                    {
                        if (resultBuffer[2] >> 4 == 0)
                        {
                            part1 = n;
                        }
                    }
                    else if (resultBuffer[2] == 0)
                    {
                        part2 = n;
                        break;
                    }
                }

                var curValue = n;
                for (var i = inputBuffer.Length - 1; i >= inputBuffer.Length - extraBytes; i--)
                {
                    if (curValue % 10 == 9)
                    {
                        inputBuffer[i] = zeroByte;
                        curValue /= 10;
                    }
                    else
                    {
                        inputBuffer[i]++;
                        break;
                    }
                }
            }

            extraBytes++;
        }

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);
    }
}
