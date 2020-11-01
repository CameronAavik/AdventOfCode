using AdventOfCode.CSharp.Y2019.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2019Tests
    {
        [Fact] public void Day1() => TestHelpers.AssertDay<Day01>("3226488", "4836845");

        [Fact] public void Day2() => TestHelpers.AssertDay<Day02>("3306701", "7621");
    }
}
