using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.CSharp.Y2015.Solvers;

public class Day07 : ISolver
{
    public void Solve(ReadOnlySpan<byte> input, Solution solution)
    {
        var rules = new Dictionary<string, string>();
        foreach (ReadOnlySpan<byte> line in input.SplitLines())
        {
            int lastSpaceIndex = line.LastIndexOf((byte)' ');
            string variableName = Encoding.ASCII.GetString(line[(lastSpaceIndex + 1)..]);
            rules[variableName] = Encoding.ASCII.GetString(line[..(lastSpaceIndex - 3)]);
        }

        var knownValues = new Dictionary<string, ushort>();

        ushort part1 = GetValue("a");

        knownValues.Clear();
        knownValues["b"] = part1;
        ushort part2 = GetValue("a");

        solution.SubmitPart1(part1);
        solution.SubmitPart2(part2);

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
