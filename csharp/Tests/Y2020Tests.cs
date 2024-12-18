﻿using AdventOfCode.CSharp.Y2020.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.CSharp.Tests;

[TestClass]
public class Y2020Tests
{
    [TestMethod] public void Day01() => TestHelpers.AssertDay<Day01>("326211", "131347190");
    [TestMethod] public void Day02() => TestHelpers.AssertDay<Day02>("660", "530");
    [TestMethod] public void Day03() => TestHelpers.AssertDay<Day03>("294", "5774564250");
    [TestMethod] public void Day04() => TestHelpers.AssertDay<Day04>("245", "133");
    [TestMethod] public void Day05() => TestHelpers.AssertDay<Day05>("813", "612");
    [TestMethod] public void Day06() => TestHelpers.AssertDay<Day06>("6170", "2947");
    [TestMethod] public void Day07() => TestHelpers.AssertDay<Day07>("372", "8015");
    [TestMethod] public void Day08() => TestHelpers.AssertDay<Day08>("1941", "2096");
    [TestMethod] public void Day09() => TestHelpers.AssertDay<Day09>("105950735", "13826915");
    [TestMethod] public void Day10() => TestHelpers.AssertDay<Day10>("2475", "442136281481216");
    [TestMethod] public void Day11() => TestHelpers.AssertDay<Day11>("2324", "2068");
    [TestMethod] public void Day12() => TestHelpers.AssertDay<Day12>("445", "42495");
    [TestMethod] public void Day13() => TestHelpers.AssertDay<Day13>("4135", "640856202464541");
    [TestMethod] public void Day14() => TestHelpers.AssertDay<Day14>("7477696999511", "3687727854171");
    [TestMethod] public void Day15() => TestHelpers.AssertDay<Day15>("758", "814");
    [TestMethod] public void Day16() => TestHelpers.AssertDay<Day16>("23036", "1909224687553");
    [TestMethod] public void Day17() => TestHelpers.AssertDay<Day17>("359", "2228");
    [TestMethod] public void Day18() => TestHelpers.AssertDay<Day18>("18213007238947", "388966573054664");
    [TestMethod] public void Day19() => TestHelpers.AssertDay<Day19>("122", "287");
    [TestMethod] public void Day20() => TestHelpers.AssertDay<Day20>("15003787688423", "1705");
    [TestMethod] public void Day21() => TestHelpers.AssertDay<Day21>("2230", "qqskn,ccvnlbp,tcm,jnqcd,qjqb,xjqd,xhzr,cjxv");
    [TestMethod] public void Day22() => TestHelpers.AssertDay<Day22>("32815", "30695");
    [TestMethod] public void Day23() => TestHelpers.AssertDay<Day23>("62934785", "693659135400");
    [TestMethod] public void Day24() => TestHelpers.AssertDay<Day24>("317", "3804");
    [TestMethod] public void Day25() => TestHelpers.AssertDay<Day25>("16311885", "");
}
