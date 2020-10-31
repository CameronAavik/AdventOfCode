using System;

namespace AdventOfCode.CSharp.Common
{
    public static class SpanExtensions
    {
        public static SpanSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> str, T separator)
            where T : IEquatable<T> => new SpanSplitEnumerator<T>(str, separator);

        public static SpanMultiSepSplitEnumerator<T> Split<T>(this ReadOnlySpan<T> str, ReadOnlySpan<T> separator)
            where T : IEquatable<T> => new SpanMultiSepSplitEnumerator<T>(str, separator);

        public ref struct SpanSplitEnumerator<T> where T : IEquatable<T>
        {
            private readonly T separator;
            private ReadOnlySpan<T> str;

            public SpanSplitEnumerator(ReadOnlySpan<T> str, T separator)
            {
                this.str = str;
                this.separator = separator;
                Current = default;
            }

            public SpanSplitEnumerator<T> GetEnumerator() => this;

            public bool MoveNext()
            {
                if (str.IsEmpty)
                {
                    return false;
                }

                var span = str;
                int index = span.IndexOf(separator);
                if (index == -1)
                {
                    str = ReadOnlySpan<T>.Empty;
                    Current = span;
                }
                else
                {
                    str = span.Slice(index + 1);
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
                MoveNext();
                return Current;
            }

            public ReadOnlySpan<T> Current { get; private set; }
        }

        public ref struct SpanMultiSepSplitEnumerator<T> where T : IEquatable<T>
        {
            private readonly ReadOnlySpan<T> splitValue;
            private ReadOnlySpan<T> str;

            public SpanMultiSepSplitEnumerator(ReadOnlySpan<T> str, ReadOnlySpan<T> splitValue)
            {
                this.str = str;
                this.splitValue = splitValue;
                Current = default;
            }

            public SpanMultiSepSplitEnumerator<T> GetEnumerator() => this;

            public bool MoveNext()
            {
                if (str.IsEmpty)
                {
                    return false;
                }

                ReadOnlySpan<T> span = str;
                int index = span.IndexOf(splitValue);
                if (index == -1)
                {
                    str = ReadOnlySpan<T>.Empty;
                    Current = span;
                }
                else
                {
                    str = span[(index + splitValue.Length)..];
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
}
