using System;

namespace AdventOfCode.CSharp.Common
{
    public interface ISolver
    {
        Solution Solve(ReadOnlySpan<byte> input);
    }
}
