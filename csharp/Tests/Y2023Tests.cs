using AdventOfCode.CSharp.Y2023.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests;

public class Y2023Tests
{
    [Fact] public void Day01() => TestHelpers.AssertDay<Day01>("55208", "54578");
    [Fact] public void Day02() => TestHelpers.AssertDay<Day02>("1734", "70387");
    [Fact] public void Day03() => TestHelpers.AssertDay<Day03>("559667", "86841457");
}
