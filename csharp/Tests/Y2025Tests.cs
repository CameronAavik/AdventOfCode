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
    [TestMethod][MultiInputData(2025, 5)] public void Day05(string filePath, string expectedPart1, string expectedPart2) => TestHelpers.AssertDay<Day05>(filePath, expectedPart1, expectedPart2);
    [TestMethod][MultiInputData(2025, 6)] public void Day06(string filePath, string expectedPart1, string expectedPart2) => TestHelpers.AssertDay<Day06>(filePath, expectedPart1, expectedPart2);
    [TestMethod][MultiInputData(2025, 7)] public void Day07(string filePath, string expectedPart1, string expectedPart2) => TestHelpers.AssertDay<Day07>(filePath, expectedPart1, expectedPart2);
    [TestMethod][MultiInputData(2025, 8)] public void Day08(string filePath, string expectedPart1, string expectedPart2) => TestHelpers.AssertDay<Day08>(filePath, expectedPart1, expectedPart2);
    [TestMethod][MultiInputData(2025, 9)] public void Day09(string filePath, string expectedPart1, string expectedPart2) => TestHelpers.AssertDay<Day09>(filePath, expectedPart1, expectedPart2);
}
