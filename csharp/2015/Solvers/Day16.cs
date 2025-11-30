using AdventOfCode.CSharp.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.CSharp.Y2015.Solvers;

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

        var sue1 = 0;
        var sue2 = 0;

        var sue = 1;
        foreach (var lineRange in input.SplitLines())
        {
            var reader = new SpanReader(input[lineRange]);
            reader.SkipUntil(':');

            var canBePart1 = true;
            var canBePart2 = true;
            for (var i = 0; i < 3; i++)
            {
                reader.SkipLength(1);
                var itemName = Encoding.ASCII.GetString(reader.ReadUntil(':'));
                reader.SkipLength(1);
                var count = reader.ReadPosIntUntil(',');

                var expectedCount = items[itemName];

                var isValidPart1 = count == expectedCount;
                var isValidPart2 = itemName switch
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
