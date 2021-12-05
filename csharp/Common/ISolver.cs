using System;

namespace AdventOfCode.CSharp.Common;

public interface ISolver
{
    void Solve(ReadOnlySpan<char> input, Solution solution);
}
