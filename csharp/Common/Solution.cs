using System;
using System.Runtime.CompilerServices;

namespace AdventOfCode.CSharp.Common;

public readonly ref struct Solution(Span<char> part1Buffer, Span<char> part2Buffer)
{
    private readonly Span<char> _part1Buffer = part1Buffer;
    private readonly Span<char> _part2Buffer = part2Buffer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubmitPart1<T>(T val) where T : ISpanFormattable
    {
        val.TryFormat(_part1Buffer, out int charsWritten, default, default);
        _part1Buffer[charsWritten] = '\n';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubmitPart2<T>(T val) where T : ISpanFormattable
    {
        val.TryFormat(_part2Buffer, out int charsWritten, default, default);
        _part2Buffer[charsWritten] = '\n';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubmitPart1(ReadOnlySpan<char> val)
    {
        val.CopyTo(_part1Buffer);
        _part1Buffer[val.Length] = '\n';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SubmitPart2(ReadOnlySpan<char> val)
    {
        val.CopyTo(_part2Buffer);
        _part2Buffer[val.Length] = '\n';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SolutionWriter GetPart1Writer() => new(_part1Buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SolutionWriter GetPart2Writer() => new(_part2Buffer);
}
