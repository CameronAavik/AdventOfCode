using AdventOfCode.CSharp.Y2016.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2016Tests
    {
        [Fact] public void Day01() => TestHelpers.AssertDay<Day01>("146", "131");
        [Fact] public void Day02() => TestHelpers.AssertDay<Day02>("45973", "27CA4");
        [Fact] public void Day03() => TestHelpers.AssertDay<Day03>("982", "1826");
        [Fact] public void Day04() => TestHelpers.AssertDay<Day04>("278221", "267");
        [Fact] public void Day05() => TestHelpers.AssertDay<Day05>("2414bc77", "437e60fc");
        [Fact] public void Day06() => TestHelpers.AssertDay<Day06>("wkbvmikb", "evakwaga");
        [Fact] public void Day07() => TestHelpers.AssertDay<Day07>("110", "242");
        [Fact] public void Day08() => TestHelpers.AssertDay<Day08>("128", "EOARGPHYAO");
        [Fact] public void Day09() => TestHelpers.AssertDay<Day09>("112830", "10931789799");
    }
}
