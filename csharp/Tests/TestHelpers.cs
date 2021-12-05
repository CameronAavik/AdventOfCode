using System;
using System.IO;
using AdventOfCode.CSharp.Common;
using Xunit;

namespace AdventOfCode.CSharp.Tests;

public static class TestHelpers
{
    internal static void AssertDay<T>(string expectedPart1, string expectedPart2)
        where T : ISolver, new()
    {
        string year = typeof(T).Namespace!.Split('.')[2][1..];
        string dayNumber = typeof(T).Name[3..];
        var solver = new T();
        ReadOnlySpan<char> file = File.ReadAllText($"input/{year}/day{dayNumber}.txt");

        Span<char> part1Buffer = new char[64];
        Span<char> part2Buffer = new char[64];
        solver.Solve(file, new(part1Buffer, part2Buffer));

        string part1 = part1Buffer.Slice(0, part1Buffer.IndexOf('\n')).ToString();
        string part2 = part2Buffer.Slice(0, part2Buffer.IndexOf('\n')).ToString();

        Assert.Equal(expectedPart1, part1);
        Assert.Equal(expectedPart2, part2);
    }
}
