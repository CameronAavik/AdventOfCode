using AdventOfCode.CSharp.Common;
using System;
using System.Text;

namespace AdventOfCode.CSharp.Y2016.Solvers;

public class Day02 : ISolver
{
    public void Solve(ReadOnlySpan<char> input, Solution solution)
    {
        const string part1Code = "123456789";
        const string part2Code = "0010002340567890ABC000E00";

        int part1x = 1, part1y = 1;
        int part2x = 2, part2y = 2;

        var part1Writer = solution.GetPart1Writer();
        var part2Writer = solution.GetPart2Writer();

        foreach (ReadOnlySpan<char> line in input.SplitLines())
        {
            foreach (char dir in line)
            {
                switch (dir)
                {
                    case 'L':
                        part1x = Math.Max(part1x - 1, 0);
                        part2x = Math.Max(part2x - 1, Math.Abs(2 - part2y));
                        break;
                    case 'R':
                        part1x = Math.Min(part1x + 1, 2);
                        part2x = Math.Min(part2x + 1, 4 - Math.Abs(2 - part2y));
                        break;
                    case 'U':
                        part1y = Math.Max(part1y - 1, 0);
                        part2y = Math.Max(part2y - 1, Math.Abs(2 - part2x));
                        break;
                    case 'D':
                        part1y = Math.Min(part1y + 1, 2);
                        part2y = Math.Min(part2y + 1, 4 - Math.Abs(2 - part2x));
                        break;
                }
            }

            part1Writer.Write(part1Code[part1x + 3 * part1y]);
            part2Writer.Write(part2Code[part2x + 5 * part2y]);
        }

        // Put a newline at the end to signify end of output
        part1Writer.Complete();
        part2Writer.Complete();
    }
}
