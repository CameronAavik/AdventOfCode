using AdventOfCode.CSharp.Y2025.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AdventOfCode.CSharp.Tests;

[TestClass]
public class Y2025Tests
{
    [TestMethod][MultiInputData(2025, 1)] public void Day01(string filePath, string expectedPart1, string expectedPart2) => TestHelpers.AssertDay<Day01>(filePath, expectedPart1, expectedPart2);
    [TestMethod][MultiInputData(2025, 2)] public void Day02(string filePath, string expectedPart1, string expectedPart2) => TestHelpers.AssertDay<Day02>(filePath, expectedPart1, expectedPart2);
    [TestMethod][MultiInputData(2025, 3)] public void Day03(string filePath, string expectedPart1, string expectedPart2) => TestHelpers.AssertDay<Day03>(filePath, expectedPart1, expectedPart2);
    [TestMethod][MultiInputData(2025, 4)] public void Day04(string filePath, string expectedPart1, string expectedPart2) => TestHelpers.AssertDay<Day04>(filePath, expectedPart1, expectedPart2);
}
