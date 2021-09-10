using System.Security.Cryptography;
using System.Text;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day04 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            const byte zeroByte = (byte)'0';
            const byte oneByte = (byte)'1';

            using var provider = MD5.Create();

            // create a buffer for the MD5 provider to store the result hash in
            Span<byte> resultBuffer = stackalloc byte[provider.HashSize / 8];

            byte[] inputAsBytes = Encoding.ASCII.GetBytes(input.TrimEnd('\n').ToArray());
            int inputLen = inputAsBytes.Length;

            int extraBytes = 1;
            int n = 1;

            int part1 = 0;
            int part2 = 0;

            // each iteration of this while loop increases the number of digits being tested
            while (part2 == 0)
            {
                // buffer that stores the data being hashed
                byte[] inputBuffer = new byte[inputLen + extraBytes];

                // populate the first bytes of the input buffer with the problem input
                inputAsBytes.CopyTo(inputBuffer, 0);

                // put the ASCII representation of '1' as the first byte and '0' for the remaining bytes
                // this means that if we were testing 5 digits, then this would start at 10000
                inputBuffer[inputLen] = oneByte;
                for (int i = inputLen + 1; i < inputBuffer.Length; i++)
                {
                    inputBuffer[i] = zeroByte;
                }

                // n will already be initialised at the correct value
                int nEnd = n * 10;
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

                    int curValue = n;
                    for (int i = inputBuffer.Length - 1; i >= inputBuffer.Length - extraBytes; i--)
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

            return new Solution(part1, part2);
        }
    }
}
