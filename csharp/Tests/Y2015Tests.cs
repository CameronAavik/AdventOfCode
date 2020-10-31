using AdventOfCode.CSharp.Y2015.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2015Tests
    {
        [Fact] public void Day1() => TestHelpers.AssertDay<Day1>("232", "1783");

        [Fact] public void Day2() => TestHelpers.AssertDay<Day2>("1586300", "3737498");
    }
}
