using AdventOfCode.CSharp.Common;
using AdventOfCode.CSharp.Y2015.Solvers;
using System;
using System.IO;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2015Tests
    {
        [Fact] public void Day1() => AssertDay<Day1>("232", "1783");

        private static void AssertDay<T>(string part1, string part2)
            where T : ISolver, new()
        {
            string dayNumber = typeof(T).Name[3..].PadLeft(2, '0');
            var solver = new T();
            ReadOnlySpan<byte> file = File.ReadAllBytes($"input/2015/day{dayNumber}.txt");
            var soln = solver.Solve(file);

            Assert.Equal(part1, soln.Part1);
            Assert.Equal(part2, soln.Part2);
        }
    }
}
