using AdventOfCode.CSharp.Y2021.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests;

public class Y2021Tests
{
    [Fact] public void Day01() => TestHelpers.AssertDay<Day01>("1167", "1130");
    [Fact] public void Day02() => TestHelpers.AssertDay<Day02>("1488669", "1176514794");
}
