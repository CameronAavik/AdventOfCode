using System;

namespace AdventOfCode.CSharp.Common;

public ref struct SpanSplitEnumerator<T>(ReadOnlySpan<T> str, T separator) where T : IEquatable<T>
{
    private readonly T _separator = separator;
    private ReadOnlySpan<T> _str = str;

    public readonly SpanSplitEnumerator<T> GetEnumerator() => this;

    public bool MoveNext()
    {
        if (_str.IsEmpty)
        {
            return false;
        }

        ReadOnlySpan<T> span = _str;
        int index = span.IndexOf(_separator);
        if (index == -1)
        {
            _str = [];
            Current = span;
        }
        else
        {
            _str = span.Slice(index + 1);
            Current = span.Slice(0, index);
        }

        return true;
    }

    public ReadOnlySpan<T> Current { get; private set; } = default;
}
