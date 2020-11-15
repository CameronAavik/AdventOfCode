using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day07 : ISolver
    {
        public Solution Solve(ReadOnlySpan<char> input)
        {
            var rules = new Dictionary<string, string>();
            foreach (ReadOnlySpan<char> line in input.Split('\n'))
            {
                int lastSpaceIndex = line.LastIndexOf(' ');
                string variableName = line[(lastSpaceIndex + 1)..].ToString();
                rules[variableName] = line[..(lastSpaceIndex - 3)].ToString();
            }

            var knownValues = new Dictionary<string, ushort>();

            ushort part1 = GetValue("a");

            knownValues.Clear();
            knownValues["b"] = part1;
            ushort part2 = GetValue("a");

            return new Solution(part1, part2);

            ushort GetValue(string variableName)
            {
                if (ushort.TryParse(variableName, out ushort value))
                {
                    return value;
                }

                if (knownValues.TryGetValue(variableName, out value))
                {
                    return value;
                }

                string rule = rules[variableName];
                int firstSpaceIndex = rule.IndexOf(' ');
                if (firstSpaceIndex == -1)
                {
                    value = GetValue(rule);
                }
                else if (rule.StartsWith("NOT"))
                {
                    value = (ushort)~GetValue(rule[4..]);
                }
                else
                {
                    ushort leftVal = GetValue(rule[..firstSpaceIndex]);
                    value = (ushort)(rule[firstSpaceIndex + 1] switch
                    {
                        'A' => leftVal & GetValue(rule[(firstSpaceIndex + 5)..]),
                        'O' => leftVal | GetValue(rule[(firstSpaceIndex + 4)..]),
                        'R' => leftVal >> GetValue(rule[(firstSpaceIndex + 8)..]),
                        'L' => leftVal << GetValue(rule[(firstSpaceIndex + 8)..]),
                        _ => 0,
                    });
                }

                return knownValues[variableName] = value;
            }
        }
    }
}
