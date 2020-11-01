using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2017.Solvers
{
    public class Day02 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            int part1 = 0;
            int part2 = 0;

            var nums = new List<int>();
            foreach (var row in input.Split('\n'))
            {
                int minValue = Int32.MaxValue;
                int maxValue = Int32.MinValue;
                int quotient = 0;

                nums.Clear();
                foreach (var cellStr in row.Split('\t'))
                {
                    int cell = Int32.Parse(cellStr);
                    minValue = Math.Min(minValue, cell);
                    maxValue = Math.Max(maxValue, cell);

                    if (quotient == 0)
                    {
                        foreach (int num in nums)
                        {
                            if (cell % num == 0)
                            {
                                quotient = cell / num;
                                break;
                            }
                            else if (num % cell == 0)
                            {
                                quotient = num / cell;
                                break;
                            }
                        }

                        nums.Add(cell);
                    }
                }

                part1 += maxValue - minValue;
                part2 += quotient;
            }

            return new Solution(part1.ToString(), part2.ToString());
        }
    }
}
