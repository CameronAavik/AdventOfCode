using System;

namespace AdventOfCode.CSharp.Common;

public interface ISolver
{
    static abstract void Solve(ReadOnlySpan<byte> input, Solution solution);
}
