using AdventOfCode.CSharp.Y2022.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests;

public class Y2022Tests
{
    [Fact] public void Day01() => TestHelpers.AssertDay<Day01>("70720", "207148");
    [Fact] public void Day02() => TestHelpers.AssertDay<Day02>("12535", "15457");
    [Fact] public void Day03() => TestHelpers.AssertDay<Day03>("8515", "2434");
    [Fact] public void Day04() => TestHelpers.AssertDay<Day04>("534", "841");
    [Fact] public void Day05() => TestHelpers.AssertDay<Day05>("JRVNHHCSJ", "GNFBSBJLH");
    [Fact] public void Day19() => TestHelpers.AssertDay<Day19>("1480", "3168");
    [Fact] public void Day20() => TestHelpers.AssertDay<Day20>("5498", "3390007892081");
}
