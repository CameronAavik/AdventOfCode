using AdventOfCode.CSharp.Y2016.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests;

public class Y2016Tests
{
    [Fact] public void Day01() => TestHelpers.AssertDay<Day01>("146", "131");
    [Fact] public void Day02() => TestHelpers.AssertDay<Day02>("45973", "27CA4");
    [Fact] public void Day03() => TestHelpers.AssertDay<Day03>("982", "1826");
    [Fact] public void Day04() => TestHelpers.AssertDay<Day04>("278221", "267");
    [Fact] public void Day05() => TestHelpers.AssertDay<Day05>("2414bc77", "437e60fc");
    [Fact] public void Day06() => TestHelpers.AssertDay<Day06>("wkbvmikb", "evakwaga");
    [Fact] public void Day07() => TestHelpers.AssertDay<Day07>("110", "242");
    [Fact] public void Day08() => TestHelpers.AssertDay<Day08>("128", "EOARGPHYAO");
    [Fact] public void Day09() => TestHelpers.AssertDay<Day09>("112830", "10931789799");
    [Fact] public void Day10() => TestHelpers.AssertDay<Day10>("86", "22847");
    [Fact] public void Day11() => TestHelpers.AssertDay<Day11>("33", "57");
    [Fact] public void Day12() => TestHelpers.AssertDay<Day12>("318003", "9227657");
    [Fact] public void Day13() => TestHelpers.AssertDay<Day13>("82", "138");
}
