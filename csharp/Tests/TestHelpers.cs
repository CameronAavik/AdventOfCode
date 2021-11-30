using System;
using System.IO;
using AdventOfCode.CSharp.Common;
using Xunit;

namespace AdventOfCode.CSharp.Tests;

public static class TestHelpers
{
    internal static void AssertDay<T>(string part1, string part2)
        where T : ISolver, new()
    {
        string year = typeof(T).Namespace!.Split('.')[2][1..];
        string dayNumber = typeof(T).Name[3..];
        var solver = new T();
        ReadOnlySpan<char> file = File.ReadAllText($"input/{year}/day{dayNumber}.txt");
        Solution soln = solver.Solve(file);

        Assert.Equal(part1, soln.Part1);
        Assert.Equal(part2, soln.Part2);
    }
}
