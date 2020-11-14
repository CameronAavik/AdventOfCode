using AdventOfCode.CSharp.Y2016.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2016Tests
    {
        [Fact] public void Day01() => TestHelpers.AssertDay<Day01>("146", "131");

        [Fact] public void Day02() => TestHelpers.AssertDay<Day02>("45973", "27CA4");
    }
}
