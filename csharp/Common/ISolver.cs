using System;

namespace AdventOfCode.CSharp.Common;

public interface ISolver
{
    void Solve(ReadOnlySpan<byte> input, Solution solution);
}
