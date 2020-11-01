using AdventOfCode.CSharp.Y2018.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2018Tests
    {
        [Fact] public void Day1() => TestHelpers.AssertDay<Day01>("516", "71892");

        [Fact] public void Day2() => TestHelpers.AssertDay<Day02>("7470", "kqzxdenujwcstybmgvyiofrrd");
    }
}
