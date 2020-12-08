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
                    var repLengthLength = input.Slice(i + 1).IndexOf('x');
                    var repLengthStr = input.Slice(i + 1, repLengthLength);
                    var repLength = int.Parse(repLengthStr);

                    var repCountLength = input.Slice(i + 2 + repLengthLength).IndexOf(')');
                    var repCountStr = input.Slice(i + 2 + repLengthLength, repCountLength);
                    var repCount = int.Parse(repCountStr);

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
                    var repLengthLength = input.Slice(i + 1).IndexOf('x');
                    var repLengthStr = input.Slice(i + 1, repLengthLength);
                    var repLength = int.Parse(repLengthStr);

                    var repCountLength = input.Slice(i + 2 + repLengthLength).IndexOf(')');
                    var repCountStr = input.Slice(i + 2 + repLengthLength, repCountLength);
                    var repCount = int.Parse(repCountStr);

                    var rep = input.Slice(i + 3 + repLengthLength + repCountLength, repLength);

                    var actualRepLength = SolvePart2(rep);
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
