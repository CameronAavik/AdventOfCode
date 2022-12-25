using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.CSharp.Y2015.Solvers
{
    public class Day16 : ISolver
    {
        public static void Solve(ReadOnlySpan<byte> input, Solution solution)
        {
            var items = new Dictionary<string, int>
            {
                ["children"] = 3,
                ["cats"] = 7,
                ["samoyeds"] = 2,
                ["pomeranians"] = 3,
                ["akitas"] = 0,
                ["vizslas"] = 0,
                ["goldfish"] = 5,
                ["trees"] = 3,
                ["cars"] = 2,
                ["perfumes"] = 1
            };

            int sue1 = 0;
            int sue2 = 0;

            int sue = 1;
            foreach (ReadOnlySpan<byte> line in input.SplitLines())
            {
                var reader = new SpanReader(line);
                reader.SkipUntil(':');

                bool canBePart1 = true;
                bool canBePart2 = true;
                for (int i = 0; i < 3; i++)
                {
                    reader.SkipLength(1);
                    string itemName = Encoding.ASCII.GetString(reader.ReadUntil(':'));
                    reader.SkipLength(1);
                    int count = reader.ReadPosIntUntil(',');

                    int expectedCount = items[itemName];

                    bool isValidPart1 = count == expectedCount;
                    bool isValidPart2 = itemName switch
                    {
                        "cats" or "trees" => count > expectedCount,
                        "pomeranians" or "goldfish" => count < expectedCount,
                        _ => count == expectedCount,
                    };

                    canBePart1 = canBePart1 && isValidPart1;
                    canBePart2 = canBePart2 && isValidPart2;
                }

                if (canBePart1)
                    sue1 = sue;

                if (canBePart2)
                    sue2 = sue;

                sue++;
            }

            solution.SubmitPart1(sue1);
            solution.SubmitPart2(sue2);
        }
    }
}
