using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Common;

public ref struct SolutionWriter(Span<char> buffer)
{
    private readonly Span<char> _buffer = buffer;
    private int _i = 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(T val) where T : ISpanFormattable
    {
        val.TryFormat(_buffer.Slice(_i), out int charsWritten, default, default);
        _i += charsWritten;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<char> val)
    {
        val.CopyTo(_buffer.Slice(_i));
        _i += val.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(char val)
    {
        _buffer[_i++] = val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Complete()
    {
        _buffer[_i] = '\n';
    }
}
