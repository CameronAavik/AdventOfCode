using AdventOfCode.CSharp.Y2015.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.CSharp.Tests;

[TestClass]
public class Y2015Tests
{
    [TestMethod] public void Day01() => TestHelpers.AssertDay<Day01>("232", "1783");
    [TestMethod] public void Day02() => TestHelpers.AssertDay<Day02>("1586300", "3737498");
    [TestMethod] public void Day03() => TestHelpers.AssertDay<Day03>("2081", "2341");
    [TestMethod] public void Day04() => TestHelpers.AssertDay<Day04>("254575", "1038736");
    [TestMethod] public void Day05() => TestHelpers.AssertDay<Day05>("238", "69");
    [TestMethod] public void Day06() => TestHelpers.AssertDay<Day06>("543903", "14687245");
    [TestMethod] public void Day07() => TestHelpers.AssertDay<Day07>("46065", "14134");
    [TestMethod] public void Day08() => TestHelpers.AssertDay<Day08>("1371", "2117");
    [TestMethod] public void Day09() => TestHelpers.AssertDay<Day09>("141", "736");
    [TestMethod] public void Day10() => TestHelpers.AssertDay<Day10>("492982", "6989950");
    [TestMethod] public void Day11() => TestHelpers.AssertDay<Day11>("vzbxxyzz", "vzcaabcc");
    [TestMethod] public void Day12() => TestHelpers.AssertDay<Day12>("191164", "87842");
    [TestMethod] public void Day13() => TestHelpers.AssertDay<Day13>("709", "668");
    [TestMethod] public void Day14() => TestHelpers.AssertDay<Day14>("2660", "1256");
    [TestMethod] public void Day15() => TestHelpers.AssertDay<Day15>("21367368", "1766400");
    [TestMethod] public void Day16() => TestHelpers.AssertDay<Day16>("213", "323");
    [TestMethod] public void Day17() => TestHelpers.AssertDay<Day17>("1638", "17");
    [TestMethod] public void Day18() => TestHelpers.AssertDay<Day18>("1061", "1006");
    [TestMethod] public void Day19() => TestHelpers.AssertDay<Day19>("518", "200");
    [TestMethod] public void Day20() => TestHelpers.AssertDay<Day20>("831600", "884520");
    [TestMethod] public void Day21() => TestHelpers.AssertDay<Day21>("121", "201");
    [TestMethod] public void Day22() => TestHelpers.AssertDay<Day22>("953", "1289");
    [TestMethod] public void Day23() => TestHelpers.AssertDay<Day23>("184", "231");
    [TestMethod] public void Day24() => TestHelpers.AssertDay<Day24>("10439961859", "72050269");
    [TestMethod] public void Day25() => TestHelpers.AssertDay<Day25>("8997277", string.Empty);
}
