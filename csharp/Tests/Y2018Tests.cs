using AdventOfCode.CSharp.Y2018.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.CSharp.Tests;

[TestClass]
public class Y2018Tests
{
    [TestMethod] public void Day01() => TestHelpers.AssertDay<Day01>("516", "71892");

    [TestMethod] public void Day02() => TestHelpers.AssertDay<Day02>("7470", "kqzxdenujwcstybmgvyiofrrd");
}
