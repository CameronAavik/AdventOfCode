using System;

namespace AdventOfCode.CSharp.Common
{
    public ref struct SpanMultiSepSplitEnumerator<T> where T : IEquatable<T>
    {
        private readonly ReadOnlySpan<T> _splitValue;
        private ReadOnlySpan<T> _str;

        public SpanMultiSepSplitEnumerator(ReadOnlySpan<T> str, ReadOnlySpan<T> splitValue)
        {
            _str = str;
            _splitValue = splitValue;
            Current = default;
        }

        public SpanMultiSepSplitEnumerator<T> GetEnumerator() => this;

        public bool MoveNext()
        {
            if (_str.IsEmpty)
            {
                return false;
            }

            ReadOnlySpan<T> span = _str;
            int index = span.IndexOf(_splitValue);
            if (index == -1)
            {
                _str = ReadOnlySpan<T>.Empty;
                Current = span;
            }
            else
            {
                _str = span[(index + _splitValue.Length)..];
                Current = span[0..index];
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

        public ReadOnlySpan<T> Current { get; private set; }
    }
}
