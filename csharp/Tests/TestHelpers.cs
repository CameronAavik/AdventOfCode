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

        Span<char> part1Buffer = new char[64];
        Span<char> part2Buffer = new char[64];
        T.Solve(file, new(part1Buffer, part2Buffer));

        int part1EndIndex = part1Buffer.IndexOf('\n');
        if (part1EndIndex == -1)
            Assert.Fail("No solution provided in part 1");

        string part1 = part1Buffer.Slice(0, part1EndIndex).ToString();
        Assert.Equal(expectedPart1, part1);

        int part2ndIndex = part2Buffer.IndexOf('\n');
        if (part2ndIndex == -1)
            Assert.Fail("No solution provided in part 2");

        string part2 = part2Buffer.Slice(0, part2ndIndex).ToString();
        Assert.Equal(expectedPart2, part2);
    }
}
