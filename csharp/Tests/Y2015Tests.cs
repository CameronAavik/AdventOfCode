using AdventOfCode.CSharp.Y2015.Solvers;
using System;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2015Tests
    {
        [Fact] public void Day01() => TestHelpers.AssertDay<Day01>("232", "1783");
        [Fact] public void Day02() => TestHelpers.AssertDay<Day02>("1586300", "3737498");
        [Fact] public void Day03() => TestHelpers.AssertDay<Day03>("2081", "2341");
        [Fact] public void Day04() => TestHelpers.AssertDay<Day04>("254575", "1038736");
        [Fact] public void Day05() => TestHelpers.AssertDay<Day05>("238", "69");
        [Fact] public void Day06() => TestHelpers.AssertDay<Day06>("543903", "14687245");
        [Fact] public void Day07() => TestHelpers.AssertDay<Day07>("46065", "14134");
        [Fact] public void Day08() => TestHelpers.AssertDay<Day08>("1371", "2117");
        [Fact] public void Day09() => TestHelpers.AssertDay<Day09>("141", "736");
        [Fact] public void Day10() => TestHelpers.AssertDay<Day10>("492982", "6989950");
        [Fact] public void Day11() => TestHelpers.AssertDay<Day11>("vzbxxyzz", "vzcaabcc");
        [Fact] public void Day12() => TestHelpers.AssertDay<Day12>("191164", "87842");
        [Fact] public void Day13() => TestHelpers.AssertDay<Day13>("709", "668");
        [Fact] public void Day14() => TestHelpers.AssertDay<Day14>("2660", "1256");
        [Fact] public void Day15() => TestHelpers.AssertDay<Day15>("21367368", "1766400");
        [Fact] public void Day16() => TestHelpers.AssertDay<Day16>("213", "323");
        [Fact] public void Day17() => TestHelpers.AssertDay<Day17>("1638", "17");
        [Fact] public void Day18() => TestHelpers.AssertDay<Day18>("1061", "1006");
        [Fact] public void Day19() => TestHelpers.AssertDay<Day19>("518", "200");
        [Fact] public void Day20() => TestHelpers.AssertDay<Day20>("831600", "884520");
        [Fact] public void Day21() => TestHelpers.AssertDay<Day21>("121", "201");
        [Fact] public void Day22() => TestHelpers.AssertDay<Day22>("953", "1289");
        [Fact] public void Day23() => TestHelpers.AssertDay<Day23>("184", "231");
        [Fact] public void Day24() => TestHelpers.AssertDay<Day24>("10439961859", "72050269");
        [Fact] public void Day25() => TestHelpers.AssertDay<Day25>("8997277", string.Empty);
    }
}
