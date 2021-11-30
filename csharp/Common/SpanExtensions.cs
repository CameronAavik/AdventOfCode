using System;

namespace AdventOfCode.CSharp.Common;

public static partial class SpanExtensions
{
    public static PermutationIterator<T> GetPermutations<T>(this Span<T> span) => new(span);

    public static SpanSplitEnumerator<char> SplitLines(this ReadOnlySpan<char> str) => new(str, '\n');

    public static SpanSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> str, T separator)
        where T : IEquatable<T> => new(str, separator);

    public static SpanMultiSepSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> str, ReadOnlySpan<T> separator)
        where T : IEquatable<T> => new(str, separator);
}
