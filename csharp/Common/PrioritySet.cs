using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AdventOfCode.CSharp.Common;

/// <summary>
/// Priority Set implementation as made by Eirik Tsarpalis:
/// https://github.com/eiriktsarpalis/pq-tests/blob/master/PriorityQueue/PrioritySet.cs
/// </summary>
/// <typeparam name="TElement"></typeparam>
/// <typeparam name="TPriority"></typeparam>
public class PrioritySet<TElement, TPriority> : IReadOnlyCollection<(TElement Element, TPriority Priority)> where TElement : notnull
{
    private const int DefaultCapacity = 4;
    private readonly Dictionary<TElement, int> _index;

    private HeapEntry[] _heap;
    private int _version;

    #region Constructors
    public PrioritySet() : this(0, null, null)
    {
    }

    public PrioritySet(int initialCapacity) : this(initialCapacity, null, null)
    {
    }

    public PrioritySet(IComparer<TPriority> comparer) : this(0, comparer, null)
    {
    }

    public PrioritySet(int initialCapacity, IComparer<TPriority>? priorityComparer, IEqualityComparer<TElement>? elementComparer)
    {
        if (initialCapacity < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException(nameof(initialCapacity));
        }

        if (initialCapacity == 0)
        {
            _heap = [];
        }
        else
        {
            _heap = new HeapEntry[initialCapacity];
        }

        _index = new Dictionary<TElement, int>(initialCapacity, comparer: elementComparer);
        Comparer = priorityComparer ?? Comparer<TPriority>.Default;
    }

    public PrioritySet(IEnumerable<(TElement Element, TPriority Priority)> values) : this(values, null, null)
    {
    }

    public PrioritySet(IEnumerable<(TElement Element, TPriority Priority)> values, IComparer<TPriority>? comparer, IEqualityComparer<TElement>? elementComparer)
    {
        Comparer = comparer ?? Comparer<TPriority>.Default;
        _index = new Dictionary<TElement, int>(elementComparer);
        _heap = [];
        Count = 0;

        AppendRaw(values);
        Heapify();
    }
    #endregion

    public int Count { get; private set; }
    public IComparer<TPriority> Comparer { get; }

    public void Enqueue(TElement element, TPriority priority)
    {
        if (_index.ContainsKey(element))
        {
            ThrowHelper.ThrowInvalidOperationException("Duplicate element");
        }

        _version++;
        Insert(in element, in priority);
    }

    public TElement Peek()
    {
        if (Count == 0)
        {
            ThrowHelper.ThrowInvalidOperationException("queue is empty");
        }

        return _heap[0].Element;
    }

    public bool TryPeek(out TElement element, out TPriority priority)
    {
        if (Count == 0)
        {
            element = default!;
            priority = default!;
            return false;
        }

        (element, priority) = _heap[0];
        return true;
    }

    public TElement Dequeue()
    {
        if (Count == 0)
        {
            ThrowHelper.ThrowInvalidOperationException("queue is empty");
        }

        _version++;
        RemoveIndex(index: 0, out TElement result, out TPriority _);
        return result;
    }

    public bool TryDequeue(out TElement element, out TPriority priority)
    {
        if (Count == 0)
        {
            element = default!;
            priority = default!;
            return false;
        }

        _version++;
        RemoveIndex(index: 0, out element, out priority);
        return true;
    }

    public TElement EnqueueDequeue(TElement element, TPriority priority)
    {
        if (Count == 0)
        {
            return element;
        }

        if (_index.ContainsKey(element))
        {
            // Set invariant validation assumes behaviour equivalent to
            // calling Enqueue(); Dequeue() operations sequentially.
            // Might consider changing to a Dequeue(); Enqueue() equivalent
            // which is more forgiving under certain scenarios.
            ThrowHelper.ThrowInvalidOperationException("Duplicate element");
        }

        ref HeapEntry minEntry = ref _heap[0];
        if (Comparer.Compare(priority, minEntry.Priority) <= 0)
        {
            return element;
        }

        _version++;
        TElement minElement = minEntry.Element;
        bool result = _index.Remove(minElement);
        Debug.Assert(result, "could not find element in index");
        SiftDown(index: 0, in element, in priority);
        return minElement;
    }

    public void Clear()
    {
        _version++;
        if (Count > 0)
        {
            //if (RuntimeHelpers.IsReferenceOrContainsReferences<HeapEntry>())
            {
                Array.Clear(_heap, 0, Count);
            }

            _index.Clear();
            Count = 0;
        }
    }

    public bool Contains(TElement element) => _index.ContainsKey(element);

    public bool TryRemove(TElement element)
    {
        if (!_index.TryGetValue(element, out int index))
        {
            return false;
        }

        _version++;
        RemoveIndex(index, out TElement _, out TPriority _);
        return true;
    }

    public bool TryUpdate(TElement element, TPriority priority)
    {
        if (!_index.TryGetValue(element, out int index))
        {
            return false;
        }

        _version++;
        UpdateIndex(index, priority);
        return true;
    }

    public void EnqueueOrUpdate(TElement element, TPriority priority)
    {
        _version++;
        if (_index.TryGetValue(element, out int index))
        {
            UpdateIndex(index, priority);
        }
        else
        {
            Insert(in element, in priority);
        }
    }

    public Enumerator GetEnumerator() => new(this);
    IEnumerator<(TElement Element, TPriority Priority)> IEnumerable<(TElement Element, TPriority Priority)>.GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public struct Enumerator : IEnumerator<(TElement Element, TPriority Priority)>, IEnumerator
    {
        private readonly PrioritySet<TElement, TPriority> _queue;
        private readonly int _version;
        private int _index;
        private (TElement Element, TPriority Priority) _current;

        internal Enumerator(PrioritySet<TElement, TPriority> queue)
        {
            _version = queue._version;
            _queue = queue;
            _index = 0;
            _current = default;
        }

        public bool MoveNext()
        {
            PrioritySet<TElement, TPriority> queue = _queue;

            if (queue._version == _version && _index < queue.Count)
            {
                ref HeapEntry entry = ref queue._heap[_index];
                _current = (entry.Element, entry.Priority);
                _index++;
                return true;
            }

            if (queue._version != _version)
            {
                ThrowHelper.ThrowInvalidOperationException("collection was modified");
            }

            return false;
        }

        public readonly (TElement Element, TPriority Priority) Current => _current;
        readonly object IEnumerator.Current => _current;

        public void Reset()
        {
            if (_queue._version != _version)
            {
                ThrowHelper.ThrowInvalidOperationException("collection was modified");
            }

            _index = 0;
            _current = default;
        }

        public readonly void Dispose()
        {
        }
    }

    #region Private Methods

    private void Heapify()
    {
        HeapEntry[] heap = _heap;

        for (int i = (Count - 1) >> 2; i >= 0; i--)
        {
            HeapEntry entry = heap[i]; // ensure struct is copied before sifting
            SiftDown(i, in entry.Element, in entry.Priority);
        }
    }

    private void Insert(in TElement element, in TPriority priority)
    {
        if (Count == _heap.Length)
        {
            PrioritySet<TElement, TPriority>.Resize(ref _heap);
        }

        SiftUp(index: Count++, in element, in priority);
    }

    private void RemoveIndex(int index, out TElement element, out TPriority priority)
    {
        Debug.Assert(index < Count);

        (element, priority) = _heap[index];

        int lastElementPos = --Count;
        ref HeapEntry lastElement = ref _heap[lastElementPos];

        if (lastElementPos > 0)
        {
            SiftDown(index, in lastElement.Element, in lastElement.Priority);
        }

        //if (RuntimeHelpers.IsReferenceOrContainsReferences<HeapEntry>())
        {
            lastElement = default;
        }

        bool result = _index.Remove(element);
        Debug.Assert(result, "could not find element in index");
    }

    private void UpdateIndex(int index, TPriority newPriority)
    {
        TElement element;
        ref HeapEntry entry = ref _heap[index];

        switch (Comparer.Compare(newPriority, entry.Priority))
        {
            // priority is decreased, sift upward
            case < 0:
                element = entry.Element; // make a copy of the element before sifting
                SiftUp(index, in element, in newPriority);
                return;

            // priority is increased, sift downward
            case > 0:
                element = entry.Element; // make a copy of the element before sifting
                SiftDown(index, in element, in newPriority);
                return;

            // priority is same as before, take no action
            default:
                return;
        }
    }

    private void AppendRaw(IEnumerable<(TElement Element, TPriority Priority)> values)
    {
        // TODO: specialize on ICollection types
        PrioritySet<TElement, TPriority>.HeapEntry[] heap = _heap;
        Dictionary<TElement, int> index = _index;
        int count = Count;

        foreach ((TElement element, TPriority priority) in values)
        {
            if (count == heap.Length)
            {
                PrioritySet<TElement, TPriority>.Resize(ref heap);
            }

            if (!index.TryAdd(element, count))
            {
                ThrowHelper.ThrowArgumentException("duplicate elements", nameof(values));
            }

            ref HeapEntry entry = ref heap[count];
            entry.Element = element;
            entry.Priority = priority;
            count++;
        }

        _heap = heap;
        Count = count;
    }

    private void SiftUp(int index, in TElement element, in TPriority priority)
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) >> 2;
            ref HeapEntry parent = ref _heap[parentIndex];

            if (Comparer.Compare(parent.Priority, priority) <= 0)
            {
                // parentPriority <= priority, heap property is satisfied
                break;
            }

            _heap[index] = parent;
            _index[parent.Element] = index;
            index = parentIndex;
        }

        ref HeapEntry entry = ref _heap[index];
        entry.Element = element;
        entry.Priority = priority;
        _index[element] = index;
    }

    private void SiftDown(int index, in TElement element, in TPriority priority)
    {
        int minChildIndex;
        int count = Count;
        HeapEntry[] heap = _heap;

        while ((minChildIndex = (index << 2) + 1) < count)
        {
            // find the child with the minimal priority
            ref HeapEntry minChild = ref heap[minChildIndex];
            int childUpperBound = Math.Min(count, minChildIndex + 4);

            for (int nextChildIndex = minChildIndex + 1; nextChildIndex < childUpperBound; nextChildIndex++)
            {
                ref HeapEntry nextChild = ref heap[nextChildIndex];
                if (Comparer.Compare(nextChild.Priority, minChild.Priority) < 0)
                {
                    minChildIndex = nextChildIndex;
                    minChild = ref nextChild;
                }
            }

            // compare with inserted priority
            if (Comparer.Compare(priority, minChild.Priority) <= 0)
            {
                // priority <= childPriority, heap property is satisfied
                break;
            }

            heap[index] = minChild;
            _index[minChild.Element] = index;
            index = minChildIndex;
        }

        ref HeapEntry entry = ref heap[index];
        entry.Element = element;
        entry.Priority = priority;
        _index[element] = index;
    }

    private static void Resize(ref HeapEntry[] heap)
    {
        int newSize = heap.Length == 0 ? DefaultCapacity : 2 * heap.Length;
        Array.Resize(ref heap, newSize);
    }

    private struct HeapEntry
    {
        public TElement Element;
        public TPriority Priority;

        public readonly void Deconstruct(out TElement element, out TPriority priority)
        {
            element = Element;
            priority = Priority;
        }
    }

#if DEBUG
    public void ValidateInternalState()
    {
        if (_heap.Length < Count)
        {
            throw new Exception("invalid elements array length");
        }

        if (_index.Count != Count)
        {
            throw new Exception("Invalid heap index count");
        }

        foreach ((TElement element, int idx) in _heap.Select((x, i) => (x.Element, i)).Skip(Count))
        {
            if (!IsDefault(element))
            {
                throw new Exception($"Non-zero element '{element}' at index {idx}.");
            }
        }

        foreach ((TPriority priority, int idx) in _heap.Select((x, i) => (x.Priority, i)).Skip(Count))
        {
            if (!IsDefault(priority))
            {
                throw new Exception($"Non-zero priority '{priority}' at index {idx}.");
            }
        }

        foreach ((TElement key, int value) in _index)
        {
            if (!_index.Comparer.Equals(_heap[value].Element, key))
            {
                throw new Exception($"Element '{key}' maps to invalid heap location {value} which contains '{_heap[value].Element}'");
            }
        }

        static bool IsDefault<T>(T value)
        {
            T defaultVal = default!;

            if (defaultVal is null)
            {
                return value is null;
            }

            return value!.Equals(defaultVal);
        }
    }
#endif
    #endregion
}
