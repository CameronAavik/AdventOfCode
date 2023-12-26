using System;
using System.IO;
using AdventOfCode.CSharp.Common;
using Xunit;

namespace AdventOfCode.CSharp.Tests;

public static class TestHelpers
{
    internal static void AssertDay<T>(string expectedPart1, string expectedPart2) where T : ISolver
    {
        string year = typeof(T).Namespace!.Split('.')[2][1..];
        string dayNumber = typeof(T).Name[3..];
        ReadOnlySpan<byte> file = File.ReadAllBytes($"input/{year}/day{dayNumber}.txt");

        char[] part1Buffer = new char[64];
        char[] part2Buffer = new char[64];
        T.Solve(file, new(part1Buffer, part2Buffer));

        // Use Assert.Multiple so I can test both part 1 and part 2 separately
        Assert.Multiple(
            () =>
            {
                int part1EndIndex = Array.IndexOf(part1Buffer, '\n');
                if (part1EndIndex == -1)
                    Assert.Fail("No solution provided in part 1");

                string part1 = part1Buffer.AsSpan().Slice(0, part1EndIndex).ToString();
                if (!part1.Equals(expectedPart1))
                    Assert.Fail($"Incorrect solution for part 1\nExpected: {expectedPart1}\nActual: {part1}");
            },
            () =>
            {
                int part2ndIndex = Array.IndexOf(part2Buffer, '\n');
                if (part2ndIndex == -1)
                    Assert.Fail("No solution provided in part 2");

                string part2 = part2Buffer.AsSpan().Slice(0, part2ndIndex).ToString();
                if (!part2.Equals(expectedPart2))
                    Assert.Fail($"Incorrect solution for part 2\nExpected: {expectedPart2}\nActual: {part2}");
            });
    }
}
