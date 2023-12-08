using AdventOfCode.CSharp.Y2023.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests;

public class Y2023Tests
{
    [Fact] public void Day01() => TestHelpers.AssertDay<Day01>("55208", "54578");
    [Fact] public void Day02() => TestHelpers.AssertDay<Day02>("1734", "70387");
    [Fact] public void Day03() => TestHelpers.AssertDay<Day03>("559667", "86841457");
    [Fact] public void Day04() => TestHelpers.AssertDay<Day04>("18519", "11787590");
    [Fact] public void Day05() => TestHelpers.AssertDay<Day05>("88151870", "2008785");
    [Fact] public void Day06() => TestHelpers.AssertDay<Day06>("2374848", "39132886");
    [Fact] public void Day07() => TestHelpers.AssertDay<Day07>("246912307", "246894760");
    [Fact] public void Day08() => TestHelpers.AssertDay<Day08>("19637", "8811050362409");
}
