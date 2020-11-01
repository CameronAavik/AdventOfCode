using AdventOfCode.CSharp.Y2017.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2017Tests
    {
        [Fact] public void Day1() => TestHelpers.AssertDay<Day01>("1047", "982");

        [Fact] public void Day2() => TestHelpers.AssertDay<Day02>("53460", "282");
    }
}
