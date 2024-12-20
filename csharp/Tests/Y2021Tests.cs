﻿using AdventOfCode.CSharp.Y2021.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.CSharp.Tests;

[TestClass]
public class Y2021Tests
{
    [TestMethod] public void Day01() => TestHelpers.AssertDay<Day01>("1167", "1130");
    [TestMethod] public void Day02() => TestHelpers.AssertDay<Day02>("1488669", "1176514794");
    [TestMethod] public void Day03() => TestHelpers.AssertDay<Day03>("738234", "3969126");
    [TestMethod] public void Day04() => TestHelpers.AssertDay<Day04>("51034", "5434");
    [TestMethod] public void Day05() => TestHelpers.AssertDay<Day05>("4993", "21101");
    [TestMethod] public void Day06() => TestHelpers.AssertDay<Day06>("375482", "1689540415957");
    [TestMethod] public void Day07() => TestHelpers.AssertDay<Day07>("355764", "99634572");
    [TestMethod] public void Day08() => TestHelpers.AssertDay<Day08>("310", "915941");
    [TestMethod] public void Day09() => TestHelpers.AssertDay<Day09>("528", "920448");
    [TestMethod] public void Day10() => TestHelpers.AssertDay<Day10>("278475", "3015539998");
    [TestMethod] public void Day11() => TestHelpers.AssertDay<Day11>("1562", "268");
    [TestMethod] public void Day12() => TestHelpers.AssertDay<Day12>("3576", "84271");
    [TestMethod] public void Day13() => TestHelpers.AssertDay<Day13>("682", "FAGURZHE");
    [TestMethod] public void Day14() => TestHelpers.AssertDay<Day14>("3306", "3760312702877");
    [TestMethod] public void Day15() => TestHelpers.AssertDay<Day15>("755", "3016");
    [TestMethod] public void Day16() => TestHelpers.AssertDay<Day16>("891", "673042777597");
    [TestMethod] public void Day17() => TestHelpers.AssertDay<Day17>("13041", "1031");
    [TestMethod] public void Day18() => TestHelpers.AssertDay<Day18>("2907", "4690");
    [TestMethod] public void Day19() => TestHelpers.AssertDay<Day19>("442", "11079");
    [TestMethod] public void Day20() => TestHelpers.AssertDay<Day20>("5464", "19228");
    [TestMethod] public void Day21() => TestHelpers.AssertDay<Day21>("929625", "175731756652760");
    [TestMethod] public void Day22() => TestHelpers.AssertDay<Day22>("588120", "1134088247046731");
    [TestMethod] public void Day23() => TestHelpers.AssertDay<Day23>("19167", "47665");
    [TestMethod] public void Day24() => TestHelpers.AssertDay<Day24>("92793949489995", "51131616112781");
    [TestMethod] public void Day25() => TestHelpers.AssertDay<Day25>("374", string.Empty);
}
