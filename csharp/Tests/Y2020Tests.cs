using AdventOfCode.CSharp.Y2020.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2020Tests
    {
        [Fact] public void Day01() => TestHelpers.AssertDay<Day01>("326211", "131347190");
        [Fact] public void Day02() => TestHelpers.AssertDay<Day02>("660", "530");
        [Fact] public void Day03() => TestHelpers.AssertDay<Day03>("294", "5774564250");
    }
}
