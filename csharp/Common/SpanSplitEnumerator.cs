using System;

namespace AdventOfCode.CSharp.Common;

public ref struct SpanSplitEnumerator<T> where T : IEquatable<T>
{
    private readonly T _separator;
    private ReadOnlySpan<T> _str;

    public SpanSplitEnumerator(ReadOnlySpan<T> str, T separator)
    {
        _str = str;
        _separator = separator;
        Current = default;
    }

    public SpanSplitEnumerator<T> GetEnumerator() => this;

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
            _str = ReadOnlySpan<T>.Empty;
            Current = span;
        }
        else
        {
            _str = span.Slice(index + 1);
            Current = span.Slice(0, index);
        }

        return true;
    }

    public ReadOnlySpan<T> Current { get; private set; }
}
