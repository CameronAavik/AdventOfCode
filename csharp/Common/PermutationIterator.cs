using System;

namespace AdventOfCode.CSharp.Common;

public ref struct PermutationIterator<T>
{
    private readonly Span<T> _span;
    private readonly int[] _stack;
    private int _stackPointer = 0;
    private bool _hasMadeFirstMove = false;

    public PermutationIterator(Span<T> span)
    {
        _span = span;
        _stack = new int[span.Length];
    }

    public readonly PermutationIterator<T> GetEnumerator() => this;

    public bool MoveNext()
    {
        // The first permutation is the input
        if (!_hasMadeFirstMove)
        {
            _hasMadeFirstMove = true;
            return true;
        }

        // iterate through all permutations using Heap's algorithm
        // https://en.wikipedia.org/wiki/Heap%27s_algorithm
        if (_stackPointer >= _stack.Length)
        {
            return false;
        }

        while (_stack[_stackPointer] >= _stackPointer)
        {
            _stack[_stackPointer++] = 0;
            if (_stackPointer >= _stack.Length)
            {
                return false;
            }
        }

        int swapIndex = _stackPointer % 2 == 0 ? 0 : _stack[_stackPointer];

        (_span[swapIndex], _span[_stackPointer]) = (_span[_stackPointer], _span[swapIndex]);
        _stack[_stackPointer]++;
        _stackPointer = 0;

        return true;
    }

    public readonly ReadOnlySpan<T> Current => _span;
}
