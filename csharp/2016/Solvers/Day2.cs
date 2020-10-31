using AdventOfCode.CSharp.Common;
using System;
using System.Text;

namespace AdventOfCode.CSharp.Y2016.Solvers
{
    public class Day2 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            const string part1Code = "123456789";
            const string part2Code = "0010002340567890ABC000E00";

            int p1x = 1, p1y = 1;
            int p2x = 2, p2y = 2;

            var part1 = new StringBuilder();
            var part2 = new StringBuilder();

            foreach (var line in input.Split('\n'))
            {
                foreach (char dir in line)
                {
                    switch (dir)
                    {
                        case 'L':
                            p1x = Math.Max(p1x - 1, 0);
                            p2x = Math.Max(p2x - 1, Math.Abs(2 - p2y));
                            break;
                        case 'R':
                            p1x = Math.Min(p1x + 1, 2);
                            p2x = Math.Min(p2x + 1, 4 - Math.Abs(2 - p2y));
                            break;
                        case 'U':
                            p1y = Math.Max(p1y - 1, 0);
                            p2y = Math.Max(p2y - 1, Math.Abs(2 - p2x));
                            break;
                        case 'D':
                            p1y = Math.Min(p1y + 1, 2);
                            p2y = Math.Min(p2y + 1, 4 - Math.Abs(2 - p2x));
                            break;
                    }
                }

                part1.Append(part1Code[p1x + 3 * p1y]);
                part2.Append(part2Code[p2x + 5 * p2y]);
            }

            return new Solution(part1.ToString(), part2.ToString());
        }
    }
}
