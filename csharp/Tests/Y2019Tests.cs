using AdventOfCode.CSharp.Y2019.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.CSharp.Tests;

[TestClass]
public class Y2019Tests
{
    [TestMethod] public void Day01() => TestHelpers.AssertDay<Day01>("3226488", "4836845");

    [TestMethod] public void Day02() => TestHelpers.AssertDay<Day02>("3306701", "7621");
}
