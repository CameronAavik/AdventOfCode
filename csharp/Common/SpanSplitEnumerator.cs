using System;

namespace AdventOfCode.CSharp.Common
{
    public static partial class SpanExtensions
    {
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

            public bool TryRead(out ReadOnlySpan<T> read)
            {
                if (MoveNext())
                {
                    read = Current;
                    return true;
                }

                read = default;
                return false;
            }

            public ReadOnlySpan<T> Read()
            {
                _ = MoveNext();
                return Current;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }
    }
}
