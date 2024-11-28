using AdventOfCode.CSharp.Y2022.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.CSharp.Tests;

[TestClass]
public class Y2022Tests
{
    [TestMethod] public void Day01() => TestHelpers.AssertDay<Day01>("70720", "207148");
    [TestMethod] public void Day02() => TestHelpers.AssertDay<Day02>("12535", "15457");
    [TestMethod] public void Day03() => TestHelpers.AssertDay<Day03>("8515", "2434");
    [TestMethod] public void Day04() => TestHelpers.AssertDay<Day04>("534", "841");
    [TestMethod] public void Day05() => TestHelpers.AssertDay<Day05>("JRVNHHCSJ", "GNFBSBJLH");
    [TestMethod] public void Day06() => TestHelpers.AssertDay<Day06>("1912", "2122");
    [TestMethod] public void Day07() => TestHelpers.AssertDay<Day07>("1141028", "8278005");
    [TestMethod] public void Day19() => TestHelpers.AssertDay<Day19>("1480", "3168");
    [TestMethod] public void Day20() => TestHelpers.AssertDay<Day20>("5498", "3390007892081");
}
