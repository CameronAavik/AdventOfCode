using System;

namespace AdventOfCode.CSharp.Common;

public static partial class SpanExtensions
{
    public static PermutationIterator<T> GetPermutations<T>(this Span<T> span) => new(span);

    public static MemoryExtensions.SpanSplitEnumerator<byte> SplitLines(this ReadOnlySpan<byte> str) => str.TrimEnd((byte)'\n').Split((byte)'\n');
}
