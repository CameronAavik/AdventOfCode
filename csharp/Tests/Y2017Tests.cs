using AdventOfCode.CSharp.Y2017.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.CSharp.Tests;

[TestClass]
public class Y2017Tests
{
    [TestMethod] public void Day01() => TestHelpers.AssertDay<Day01>("1047", "982");

    [TestMethod] public void Day02() => TestHelpers.AssertDay<Day02>("53460", "282");
}
