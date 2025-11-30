using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AdventOfCode.CSharp.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: DoNotParallelize]

namespace AdventOfCode.CSharp.Tests;

public static class TestHelpers
{
    internal static void AssertDay<T>(string expectedPart1, string expectedPart2) where T : ISolver
    {
        var year = typeof(T).Namespace!.Split('.')[2][1..];
        var dayNumber = typeof(T).Name[3..];
        AssertDay<T>($"input/{year}/day{dayNumber}.txt", expectedPart1, expectedPart2);
    }

    internal static void AssertDay<T>(string filePath, string expectedPart1, string expectedPart2) where T : ISolver
    {
        ReadOnlySpan<byte> file = File.ReadAllBytes(filePath);

        var part1Buffer = new char[64];
        var part2Buffer = new char[64];
        T.Solve(file, new(part1Buffer, part2Buffer));

        List<string> errorMessages = [];

        var part1EndIndex = Array.IndexOf(part1Buffer, '\n');
        if (part1EndIndex == -1)
        {
            errorMessages.Add("No solution provided in part 1");
        }
        else
        {
            var part1 = part1Buffer.AsSpan()[..part1EndIndex].ToString();
            if (!part1.Equals(expectedPart1))
                errorMessages.Add($"Incorrect solution for part 1\nExpected: {expectedPart1}\nActual: {part1}");
        }

        var part2EndIndex = Array.IndexOf(part2Buffer, '\n');
        if (part2EndIndex == -1)
        {
            errorMessages.Add("No solution provided in part 2");
        }
        else
        {
            var part2 = part2Buffer.AsSpan()[..part2EndIndex].ToString();
            if (!part2.Equals(expectedPart2))
                errorMessages.Add($"Incorrect solution for part 2\nExpected: {expectedPart2}\nActual: {part2}");
        }

        if (errorMessages.Count > 0)
            Assert.Fail(string.Join("\n", errorMessages));
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class MultiInputDataAttribute(int year, int day) : Attribute, ITestDataSource
{
    public IEnumerable<object[]> GetData(MethodInfo methodInfo)
    {
        var inputFolder = $"input/{year}/extra/day{day:D2}";
        foreach (var file in Directory.EnumerateFiles(inputFolder))
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var parts = fileName.Split('_');
            yield return new object[] { file, parts[0], parts[1] };
        }
    }

    public string? GetDisplayName(MethodInfo methodInfo, object?[]? data) => $"{data![1]}_{data![2]}";
}
