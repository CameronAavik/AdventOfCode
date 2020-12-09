using System;
using AdventOfCode.CSharp.Common;

namespace AdventOfCode.CSharp.Y2016.Solvers
{
    public class Day09 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            input = input.TrimEnd('\n');
            int part1 = SolvePart1(input);
            long part2 = SolvePart2(input);

            return new Solution(part1.ToString(), part2.ToString());
        }

        private static int SolvePart1(ReadOnlySpan<char> input)
        {
            int length = 0;
            int i = 0;
            while (i < input.Length)
            {
                if (input[i] == '(')
                {
                    int repLengthLength = input.Slice(i + 1).IndexOf('x');
                    ReadOnlySpan<char> repLengthStr = input.Slice(i + 1, repLengthLength);
                    int repLength = int.Parse(repLengthStr);

                    int repCountLength = input.Slice(i + 2 + repLengthLength).IndexOf(')');
                    ReadOnlySpan<char> repCountStr = input.Slice(i + 2 + repLengthLength, repCountLength);
                    int repCount = int.Parse(repCountStr);

                    length += repLength * repCount;
                    i += 3 + repLengthLength + repCountLength + repLength;
                }
                else
                {
                    length += 1;
                    i++;
                }
            }

            return length;
        }

        private static long SolvePart2(ReadOnlySpan<char> input)
        {
            long length = 0;
            int i = 0;
            while (i < input.Length)
            {
                if (input[i] == '(')
                {
                    int repLengthLength = input.Slice(i + 1).IndexOf('x');
                    ReadOnlySpan<char> repLengthStr = input.Slice(i + 1, repLengthLength);
                    int repLength = int.Parse(repLengthStr);

                    int repCountLength = input.Slice(i + 2 + repLengthLength).IndexOf(')');
                    ReadOnlySpan<char> repCountStr = input.Slice(i + 2 + repLengthLength, repCountLength);
                    int repCount = int.Parse(repCountStr);

                    ReadOnlySpan<char> rep = input.Slice(i + 3 + repLengthLength + repCountLength, repLength);

                    long actualRepLength = SolvePart2(rep);
                    length += actualRepLength * repCount;
                    i += 3 + repLengthLength + repCountLength + repLength;
                }
                else
                {
                    length += 1;
                    i++;
                }
            }

            return length;
        }
    }
}
