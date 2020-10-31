﻿using AdventOfCode.CSharp.Y2016.Solvers;
using Xunit;

namespace AdventOfCode.CSharp.Tests
{
    public class Y2016Tests
    {
        [Fact] public void Day1() => TestHelpers.AssertDay<Day1>("146", "131");

        [Fact] public void Day2() => TestHelpers.AssertDay<Day2>("45973", "27CA4");
    }
}
