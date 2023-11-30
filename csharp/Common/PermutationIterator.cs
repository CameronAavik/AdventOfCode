using System;

namespace AdventOfCode.CSharp.Common;

public ref struct PermutationIterator<T>(Span<T> span)
{
    private bool HasMadeFirstMove { get; set; } = false;

    private int StackPointer { get; set; } = 0;

    private int[] Stack { get; set; } = new int[span.Length];

    public readonly PermutationIterator<T> GetEnumerator() => this;

    public bool MoveNext()
    {
        // The first permutation is the input
        if (!HasMadeFirstMove)
        {
            HasMadeFirstMove = true;
            return true;
        }

        // iterate through all permutations using Heap's algorithm
        // https://en.wikipedia.org/wiki/Heap%27s_algorithm
        if (StackPointer >= Stack.Length)
        {
            return false;
        }

        while (Stack[StackPointer] >= StackPointer)
        {
            Stack[StackPointer++] = 0;
            if (StackPointer >= Stack.Length)
            {
                return false;
            }
        }

        int swapIndex = StackPointer % 2 == 0 ? 0 : Stack[StackPointer];

        (Current[swapIndex], Current[StackPointer]) = (Current[StackPointer], Current[swapIndex]);
        Stack[StackPointer]++;
        StackPointer = 0;

        return true;
    }

    public Span<T> Current { get; } = span;
}
