
namespace ZuList
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using ZuList.Internal;

    [DebuggerTypeProxy(typeof(FastListDebugView<>))]
    [DebuggerDisplay("Count = {Count} Capacity = {Capacity}")]
    [Serializable]
    public partial class FastList<T> : IList<T>, IList, IReadOnlyList<T>
    {
        private const int _defaultCapacity = 4;
        private static readonly T[] _empty = Array.Empty<T>();

        // List<T> binary serialization
        private T[] _items;
        private int _size;
        private int _version;

        public FastList()
        {
            _items = _empty;
            _size = 0;
        }

#pragma warning disable CS8618
        public FastList(FastList<T> fastList)
        {
            ErrorHelper.ThrowArgumentNullException(fastList, nameof(fastList));

            var argsListSize = fastList._size;
            if (argsListSize == 0)
            {
                _items = _empty;
                _size = 0;
                return;
            }

            _items = new T[argsListSize];

            // memory copying of arrays
            var byteCount = UnsafeSize<T>.value * argsListSize;
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(fastList._items)!);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(_items)!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)byteCount);

            _size = fastList._size;
        }
#pragma warning restore CS8618

        public FastList(ICollection<T> collection)
        {
            ErrorHelper.ThrowArgumentNullException(collection, nameof(collection));

            int count = collection.Count;
            if (count == 0)
            {
                _items = _empty;
                _size = 0;
            }
            else
            {
                _items = new T[count];
                collection.CopyTo(_items, 0);
                _size = count;
            }
        }

        public FastList(IEnumerable<T> collection)
        {
            ErrorHelper.ThrowArgumentNullException(collection, nameof(collection));

            _size = 0;
            _items = _empty;
            using IEnumerator<T> en = collection!.GetEnumerator();
            while (en.MoveNext())
            {
                Add(en.Current);
            }
        }

        public FastList(int capacity)
        {
            ErrorHelper.ThrowArgumentOutOfRangeExceptionLessThanZero(capacity, nameof(capacity));
            _items = capacity == 0 ? _empty : new T[capacity];
            _size = 0;
        }

        #region "FastList<T> and List<T> compatible"

        public int Capacity
        {
            get => _items.Length;
            set
            {
                if (value < _size) return;

                if (value <= 0)
                {
                    _items = _empty;
                    _size = 0;
                    return;
                }

                this.DangerousEnsureCapacity(value);
            }
        }

        public bool IsEmpty => _size != 0;

        public ReadOnlyCollection<T> AsReadOnly()
            => new(this);

        public int BinarySearch(T item)
            => this.BinarySearch(0, _size, item, null);

        public int BinarySearch(T item, IComparer<T>? comparer)
            => this.BinarySearch(0, _size, item, comparer);

        public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
        {
            ErrorHelper.ThrowArgumentOutOfRangeExceptionLessThanZero(index, "index is less than 0.");
            ErrorHelper.ThrowArgumentOutOfRangeExceptionLessThanZero(index, "count is less than 0.");
            if (_size - index < count)
                ErrorHelper.ThrowArgumentException("index and count do not denote a valid range in the FastList<T>.");

            return Array.BinarySearch(_items, index, count, item, comparer);
        }

        public FastList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            ErrorHelper.ThrowArgumentNullException(converter, nameof(converter));

            var list = new FastList<TOutput>(_size);
            ConvertAllSpan(ref converter, _items.AsSpan(0, _size), list._items.AsSpan());
            list._size = _size;
            return list;
        }

        private static void ConvertAllSpan<TOutput>(ref Converter<T, TOutput> converter, Span<T> itemSpan, Span<TOutput> outputSpan)
        {
            for (int i = 0; i < itemSpan.Length; i++)
            {
                outputSpan[i] = converter(itemSpan[i]);
            }
        }

        public int EnsureCapacity(int capacity)
        {
            if (_items.Length < capacity)
            {
                this.DangerousEnsureCapacity(this.CalculateEnsureCapacity(capacity));
            }
            return _items.Length;
        }

        public int ShrinkToFitCapacity() 
        {
            var size = _size;
            if (size == 0)
            {
                _items = _empty;
                return size;
            }

            if (size < _items.Length)
            {
                this.DangerousEnsureCapacity(size);
            }
            
            return size;
        }

        public void AddRange(FastList<T> fastList)
        {
            ErrorHelper.ThrowArgumentNullException(fastList, nameof(fastList));
            if (fastList._size == 0) return;

            var size = _size;
            var argsFastListSize = fastList._size;
            var addedSize = size + argsFastListSize;
            if (_items.Length < addedSize)
            {
                this.DangerousEnsureCapacity(this.CalculateEnsureCapacity(addedSize));
            }

            DangerousCopyArray(_items.AsSpan(size, argsFastListSize), fastList._items);
            _size = addedSize;
            _version++;
        }

        public void AddRange(T[] array)
        {
            ErrorHelper.ThrowArgumentNullException(array, nameof(array));
            if (array.Length == 0) return;

            var size = _size;
            var argsArraySize = array.Length;
            var addedSize = size + argsArraySize;
            if (_items.Length < addedSize)
            {
                this.DangerousEnsureCapacity(this.CalculateEnsureCapacity(addedSize));
            }

            DangerousCopyArray(_items.AsSpan(size, argsArraySize), array);
            _size = addedSize;
            _version++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DangerousCopyArray(Span<T> destItem, T[] sourceItemArray)
        {
            // additem memory copying of arrays
            var addItemByteCount = UnsafeSize<T>.value * destItem.Length;
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(sourceItemArray)!);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destItem)!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)addItemByteCount);
        }

        public void AddRange(ICollection<T> collection)
        {
            ErrorHelper.ThrowArgumentNullException(collection, nameof(collection));
            if (collection.Count == 0) return;

            var size = _size;
            var addedSize = size + collection.Count;
            if (_items.Length < addedSize)
            {
                this.DangerousEnsureCapacity(this.CalculateEnsureCapacity(addedSize));
            }

            collection.CopyTo(_items, _size);
            _size = addedSize;
            _version++;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            ErrorHelper.ThrowArgumentNullException(collection, nameof(collection));

            if (collection is ICollection<T>)
            {
                this.AddRange((collection as ICollection<T>)!);
            }
            else
            {
                using IEnumerator<T> en = collection!.GetEnumerator();
                while (en.MoveNext())
                {
                    Add(en.Current);
                }
            }
        }

        public void InsertRange(int index, FastList<T> fastList)
        {
            ErrorHelper.ThrowArgumentNullException(fastList, nameof(fastList));
            if ((uint)_size < (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));
            if (fastList._size == 0) return;

            var size = _size;
            var insertedSize = size + fastList._size;
            if (_items.Length < insertedSize)
            {
                this.DangerousEnsureCapacity(this.CalculateEnsureCapacity(insertedSize));
            }

            // end index is AddRange
            if (size == index)
            {
                this.AddRange(fastList);
                return;
            }

            var moveItemSpan = _items.AsSpan(index, _size - index);
            var insertItemSpan = fastList._items.AsSpan(0, fastList._size);

            this.DangerousInsertRange(index, moveItemSpan, insertItemSpan);
            _size = insertedSize;
            _version++;
        }

        public void InsertRange(int index, T[] array)
        {
            ErrorHelper.ThrowArgumentNullException(array, nameof(array));
            if ((uint)_size < (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));
            if (array.Length == 0) return;

            var size = _size;
            var insertedSize = size + array.Length;
            if (_items.Length < insertedSize)
            {
                this.DangerousEnsureCapacity(this.CalculateEnsureCapacity(insertedSize));
            }

            // end index is AddRange
            if (size == index)
            {
                this.AddRange(array);
                return;
            }

            var moveItemSpan = _items.AsSpan(index, _size - index);
            var insertItemSpan = array.AsSpan();

            this.DangerousInsertRange(index, moveItemSpan, insertItemSpan);
            _size = insertedSize;
            _version++;
        }

        public void InsertRange(int index, ICollection<T> collection)
        {
            ErrorHelper.ThrowArgumentNullException(collection, nameof(collection));
            if ((uint)_size < (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));
            if (collection.Count == 0) return;

            var size = _size;
            var insertedSize = size + collection.Count;
            if (_items.Length < insertedSize)
            {
                this.DangerousEnsureCapacity(this.CalculateEnsureCapacity(insertedSize));
            }

            // end index is AddRange
            if( size == index)
            {
                this.AddRange(collection);
                return;
            }
            var moveItemSpan = _items.AsSpan(index, _size - index);

            // moveItem memory copying of arrays
            var moveItemByteCount = UnsafeSize<T>.value * moveItemSpan.Length;
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(moveItemSpan)!);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(_items.AsSpan(index + collection.Count, _size - index))!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)moveItemByteCount);

            collection.CopyTo(_items, index);

            _size = insertedSize;
            _version++;
        }

        private void DangerousInsertRange(int index, Span<T> moveItemSpan, Span<T> insertItemSpan)
        {
            // moveItem memory copying of arrays
            var moveItemByteCount = UnsafeSize<T>.value * moveItemSpan.Length;
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(moveItemSpan)!);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(_items.AsSpan(index + insertItemSpan.Length, _size - index))!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)moveItemByteCount);

            // insertitem memory copying of arrays
            var insertItemByteCount = UnsafeSize<T>.value * insertItemSpan.Length;
            ref var source2 = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(insertItemSpan)!);
            ref var dest2 = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(_items.AsSpan(index, insertItemSpan.Length))!);
            Unsafe.CopyBlockUnaligned(ref dest2, ref source2, (uint)insertItemByteCount);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            ErrorHelper.ThrowArgumentNullException(collection, nameof(collection));
            if ((uint)_size < (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));

            using IEnumerator<T> en = collection!.GetEnumerator();
            while (en.MoveNext())
            {
                Insert(index++, en.Current);
            }
        }

        public void RemoveRange(int index, int count)
        {
            if ((uint)_size < (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));
            if (count < 0 && _size - count < index) ErrorHelper.ThrowArgumentOutOfRangeException("");

            // End index
            if (index < _size - count)
            {
                var items = _items;
                var size = _size;
                var sourceItemSpan = items.AsSpan(index, size - (index + count));
                var moveItemSpan = items.AsSpan(index + count, size - (index + count));

                this.RemoveSpan(sourceItemSpan, moveItemSpan);
            }

            _size -= count;
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _items.AsSpan(_size, count).Clear();
            }
            _version++;
        }

        public int RemoveAll(Predicate<T> match)
        {
            ErrorHelper.ThrowArgumentNullException(match, nameof(match));

            int removedIndex = 0;
            var result = this.RemoveAllSpan(ref match, _items.AsSpan(0, _size), ref removedIndex);
            if (result == 0) return result;

            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _items.AsSpan(removedIndex, _size - removedIndex).Clear();
            }
            _size = removedIndex;
            _version++;

            return result;
        }

        private int RemoveAllSpan( ref Predicate<T> match, Span<T> items, ref int removedIndex)
        {
            while (removedIndex < _size && !match(items[removedIndex]))
            {
                removedIndex++;
            }
            if (removedIndex >= _size) return 0;

            int current = removedIndex + 1;
            while (current < _size)
            {
                while (current < _size && match(items[current])) current++;

                if (current < _size)
                {
                    items[removedIndex++] = items[current++];
                }
            }
            return _size - removedIndex;
        }

        public bool Exists(Predicate<T> match)
            => this.FindIndex(match) > -1;

        public T? Find(Predicate<T> match)
        {
            ErrorHelper.ThrowArgumentNullException(match, nameof(match));

            return FastList<T>.FindSpan(ref match, _items.AsSpan(0, _size));
        }

        private static T? FindSpan(ref Predicate<T> match, Span<T> itemSpan)
        {
            for (int i = 0; i < itemSpan.Length; i++)
            {
                if (match(itemSpan[i]))
                {
                    return itemSpan[i];
                }
            }
            return default;
        }

        public int FindIndex(Predicate<T> match)
            => FindIndex(0, _size, match);

        public int FindIndex(int startIndex, Predicate<T> match)
            => FindIndex(startIndex, _size - startIndex, match);

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            ErrorHelper.ThrowArgumentNullException(match, nameof(match));
            if ((uint)_size < (uint)startIndex) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0 && _size - count < startIndex) ErrorHelper.ThrowArgumentOutOfRangeException("");

            return FastList<T>.FindIndexSpan(ref match, _items.AsSpan(startIndex, count), startIndex);
        }

        private static int FindIndexSpan(ref Predicate<T> match, Span<T> itemSpan, int startIndex)
        {
            for (int i = 0; i < itemSpan.Length; i++)
            {
                if (match(itemSpan[i])) return i + startIndex;
            }
            return -1;
        }

        public T? FindLast(Predicate<T> match)
        {
            ErrorHelper.ThrowArgumentNullException(match, nameof(match));

            return FastList<T>.FindLastSpan(ref match, _items.AsSpan(0, _size));
        }

        private static T? FindLastSpan(ref Predicate<T> match, Span<T> itemSpan)
        {
            for (int i = itemSpan.Length - 1; i >= 0; i--)
            {
                if (match(itemSpan[i]))
                {
                    return itemSpan[i];
                }
            }
            return default;
        }
        
        public int FindLastIndex(Predicate<T> match)
            => FindLastIndex(_size - 1, _size, match);
        
        public int FindLastIndex(int startIndex, Predicate<T> match)
            => FindLastIndex(startIndex, startIndex + 1, match);

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            ErrorHelper.ThrowArgumentNullException(match, nameof(match));
            if ((uint)_size < (uint)startIndex) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0 && _size - count < startIndex) ErrorHelper.ThrowArgumentOutOfRangeException("");

            var endIndex = startIndex - count + 1;
            return FastList<T>.FindLastIndexSpan(ref match, _items.AsSpan(endIndex, count), endIndex);
        }

        private static int FindLastIndexSpan(ref Predicate<T> match, Span<T> itemSpan, int endIndex)
        {
            for (int i = itemSpan.Length - 1; i >= 0; i--)
            {
                if (match(itemSpan[i]))
                {
                    return i + endIndex;
                }
            }
            return -1;
        }

        public FastList<T> FindAll(Predicate<T> match)
        {
            ErrorHelper.ThrowArgumentNullException(match, nameof(match));

            return FastList<T>.FindAllSpan(ref match, _items.AsSpan(0, _size));
        }

        private static FastList<T> FindAllSpan(ref Predicate<T> match, Span<T> itemSpan)
        {
            var fastList = new FastList<T>();
            for (int i = 0; i < itemSpan.Length; i++)
            {
                if (match(itemSpan[i]))
                {
                    fastList.Add(itemSpan[i]);
                }
            }
            return fastList;
        }

        public void ForEach(Action<T> action)
        {
            ErrorHelper.ThrowArgumentNullException(action, nameof(action));

            FastList<T>.ForEachSpan(ref action, _items.AsSpan(0, _size));
        }

        private static void ForEachSpan(ref Action<T> action, Span<T> itemSpan)
        {
            for (int i = 0; i < itemSpan.Length; i++)
            {
                action(itemSpan[i]);
            }
        }

        public FastList<T> GetRange(int index, int count)
        {
            if ((uint)_size < (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));
            if (count < 0 && _size - count < index) ErrorHelper.ThrowArgumentOutOfRangeException("");

            var fastList = new FastList<T>(count);

            // memory copying of arrays
            var byteCount = UnsafeSize<T>.value * count;
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(_items.AsSpan(index, count))!);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(fastList._items)!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)byteCount);
            fastList._size = count;
            return fastList;   
        }

        public void Reverse()
            => this.Reverse(0, _size);

        public void Reverse(int index, int count)
        {
            if (index < 0) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));
            if (count < 0) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(count));
            if (_size - index < count) ErrorHelper.ThrowArgumentOutOfRangeException("");
            if (_size == 0) return;

            MemoryExtensions.Reverse(_items.AsSpan(index, count));
            _version++;  
        }

        public FastList<T> Slice(int start, int length)
            => this.GetRange(start, length);

        public bool TrueForAll(Predicate<T> match)
        {
            ErrorHelper.ThrowArgumentNullException(match, nameof(match));

            return TrueForAllSpan(ref match, _items.AsSpan(0, _size));
        }

        private static bool TrueForAllSpan(ref Predicate<T> match, Span<T> itemsSpan)
        {
            for (int i = 0; i < itemsSpan.Length; i++)
            {
                if (!match(itemsSpan[i]))
                    return false;
            }
            return true;
        }

        public T[] ToArray()
        {
            if (_size == 0)
            {
                return _empty;
            }

            var array = new T[_size];

            // memory copying of arrays
            var byteCount = UnsafeSize<T>.value * array.Length;
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(array)!);
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(_items)!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)byteCount);

            return array;
        }

        public string ToDebugString()
        {
            if (_size == 0) return "[ ]";

            var builder = new System.Text.StringBuilder();
            var arraySpan = _items.AsSpan(0, _size);
            builder.Append('[');
            for (var i = 0; i < arraySpan.Length - 1; i++)
            {
                builder.Append(' ');
                builder.Append(arraySpan[i]?.ToString() ?? "<null>");
                builder.Append(',');
            }
            builder.Append(' ');
            builder.Append(arraySpan[arraySpan.Length - 1]?.ToString() ?? "<null>");
            builder.Append(' ');
            builder.Append(']');
            return builder.ToString();
        }

        public List<T> ToList()
        {
            var list = new List<T>();
            var copy = Unsafe.As<List<T>, ListCopy>(ref list);

            var argsListArray = _items.AsSpan(0, _size);
            copy._items = new T[argsListArray.Length];
            copy._size = _size;
            copy._version = _version;

            // memory copying of arrays
            var byteCount = UnsafeSize<T>.value * argsListArray.Length;
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(argsListArray)!);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(copy._items)!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)byteCount);

            return list;
        }

        public void TrimExcess()
        {
            var thresold = (int)(_items.Length * 0.9);
            if (_size < thresold)
            {
                this.DangerousEnsureCapacity(_size);
            }
        }

        public void Sort()
            => this.Sort(0, _size, null);

        public void Sort(IComparer<T>? comparer)
            => this.Sort(0, _size, comparer);

        public void Sort(int index, int count, IComparer<T>? comparer)
        {
            Array.Sort(_items, index, count, comparer);
            _version++;
        }

        public void Sort(Comparison<T> comparison)
        {
            _items.AsSpan(0, _size).Sort(comparison);
            _version++;
        }

        #endregion

        #region "IList<T>"

        public T this[int index] 
        {
            get
            {
                if ((uint)_size <= (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));
                return _items[index];
            }
            set
            {
                if ((uint)_size <= (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));
                _items[index] = value;
                _version++;
            }
        }

        public int Count => _size;

        public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            _version++;
            var size = _size;
            var items = _items;
            if((uint)size < (uint)items.Length)
            {
                _size = size + 1;
                items[size] = item;
            }
            else
            {
                this.AddAndEnsureCapacity(item);
            }
        }

        public void Clear()
        {
            _version++;
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                if (_size > 0)
                {
                    _items.AsSpan(0, _size).Clear(); ;
                }
            }
            _size = 0;
        }

        public bool Contains(T item) 
            => _size != 0 && this.IndexOf(item) >= 0;

        public void CopyTo(T[] array)
            => this.CopyTo(array, 0);

        public void CopyTo(T[] array, int arrayIndex)
        {
            ErrorHelper.ThrowArgumentNullException(array, nameof(array));

            if (_size == 0) return;
            if (array.Length == 0) return;
            if (array.Length < _size)
                ErrorHelper.ThrowArgumentException(nameof(array));

            var destArraySpan = array.AsSpan(arrayIndex, _size);
            var sourceArraySpan = _items.AsSpan(0, _size);

            // memory copying of arrays
            var byteCount = UnsafeSize<T>.value * destArraySpan.Length;
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(sourceArraySpan)!);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destArraySpan)!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)byteCount);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            ErrorHelper.ThrowArgumentNullException(array, nameof(array));
            ErrorHelper.ThrowNoElementInArray(array);

            if (_size == 0) return;
            if (array.Length == 0) return;
            if (array.Length < index + count)
                ErrorHelper.ThrowArgumentOutOfRangeException(nameof(array));
            if (_items.Length < arrayIndex + count)
                ErrorHelper.ThrowArgumentOutOfRangeException(nameof(array));

            var destArraySpan = array.AsSpan(index, count);
            var sourceArraySpan = _items.AsSpan(arrayIndex, count);

            // memory copying of arrays
            var byteCount = UnsafeSize<T>.value * destArraySpan.Length;
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(sourceArraySpan)!);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(destArraySpan)!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)byteCount);
        }

        public IEnumerator<T> GetEnumerator()
            => new Enumerator(this);

        public int IndexOf(T item)
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                return FastList<T>.IndexOfEqualityCompare(_items.AsSpan(0, _size), item, 0);
            }
            
            return Array.IndexOf(_items, item, 0, _size);
        }

        private static int IndexOfEqualityCompare(Span<T> arraySpan, T value, int startIndex)
        {
            if (value != null)
            {
                for (int i = 0; i < arraySpan.Length; i++)
                {
                    if (arraySpan[i] != null && arraySpan[i]!.Equals(value))
                        return i + startIndex;
                }
            }
            else
            {
                for (int i = 0; i < arraySpan.Length; i++)
                {
                    if (arraySpan[i] == null)
                        return i + startIndex;
                }
            }
            return -1;
        }


        public void Insert(int index, T item)
        {
            ErrorHelper.ThrowArgumentNullException(item, nameof(item));
            if ((uint)_size < (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));

            this.Add(item);
            
            var addedSize = _size;
            if (addedSize == index + 1) return;

            var items = _items;
            var moveRangeCount = addedSize - (index + 1);

            // memory copying of arrays
            var byteCount = UnsafeSize<T>.value * moveRangeCount;
            ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(items.AsSpan(index, moveRangeCount))!);
            ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetReference(items.AsSpan(index + 1, moveRangeCount))!);
            Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)byteCount);

            _items[index] = item;
        }

        public bool Remove(T item)
        {
            var index = this.IndexOf(item);
            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }

            return false;
        }

        public void RemoveAt(int index)
        {
            if ((int)_size <= (uint)index) ErrorHelper.ThrowArgumentOutOfRangeException(nameof(index));

            if (index < _size - 1)
            {
                var items = _items;
                var length = _size;
                var sourceItemSpan = items.AsSpan(index, length - (index + 1));
                var moveItemSpan = items.AsSpan(index + 1, length - (index + 1));

                this.RemoveSpan(sourceItemSpan, moveItemSpan);
            }
            _size--;
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _items[_size] = default!;
            }
            _version++;
        }

        private void RemoveSpan(Span<T> sourceItemSpan, Span<T> moveItemSpan)
        {
            for (var i = 0; i < moveItemSpan.Length; i++)
            {
                sourceItemSpan[i] = moveItemSpan[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => new Enumerator(this);

        #endregion

        #region "IList"

        public bool IsFixedSize => false;

        public bool IsSynchronized => false;

        public object SyncRoot => this;

        object? IList.this[int index] 
        { 
            get => this[index];
            set
            {
                ErrorHelper.ThrowArgumentExceptionIfNullAndNullsAreIlleagal<T>(value!, nameof(value));
                ErrorHelper.ThrowArgumentExceptionIfWrongValueType<T>(value!);
                this[index] = Unsafe.As<object, T>(ref value!);
            }
        }

        public int Add(object? value)
        {
            ErrorHelper.ThrowArgumentExceptionIfNullAndNullsAreIlleagal<T>(value!, nameof(value));
            ErrorHelper.ThrowArgumentExceptionIfWrongValueType<T>(value!);

            this.Add(Unsafe.As<object, T>(ref value!));
            return _size - 1;
        }

        public bool Contains(object? value)
        {
            if (!FastList<T>.IsConvertibleObject(value!)) return false;

            return this.Contains(Unsafe.As<object, T>(ref value!));
        }

        public int IndexOf(object? value)
        {
            if (!FastList<T>.IsConvertibleObject(value!)) return -1;

            return this.IndexOf(Unsafe.As<object, T>(ref value!));
        }

        public void Insert(int index, object? value)
        {
            ErrorHelper.ThrowArgumentExceptionIfNullAndNullsAreIlleagal<T>(value!, nameof(value));
            ErrorHelper.ThrowArgumentExceptionIfWrongValueType<T>(value!);

            this.Insert(index, Unsafe.As<object, T>(ref value!));
        }

        public void Remove(object? value)
        {
            if (FastList<T>.IsConvertibleObject(value!))
            {
                this.Remove(Unsafe.As<object, T>(ref value!));
            }
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            ErrorHelper.ThrowArgumentNullException(array, nameof(array));
            ErrorHelper.ThrowNoElementInArray(array);
            ErrorHelper.ThrowMultidimensional(array);

            if (array.Length < _size)
                ErrorHelper.ThrowArgumentException(nameof(array));

            Array.Copy(_items, 0, array, arrayIndex, _size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsConvertibleObject(object value)
            => value is T || value is null;

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateEnsureCapacity(int capacity)
            => Math.Max(_items.Length == 0 ? _defaultCapacity : Math.Min(_items.Length * 2, Array.MaxLength), capacity);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddAndEnsureCapacity(T item)
        {
            var size = _size;
            var items = _items;
            int newCapacity;
            if (items.Length == 0)
            {
                newCapacity = _defaultCapacity;
            }
            else
            {
                newCapacity = Math.Min(items.Length * 2, Array.MaxLength);
            }

            this.DangerousEnsureCapacity(newCapacity);
            _size = size + 1;
            _items[size] = item;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DangerousEnsureCapacity(int capacity)
        {
            var newItems = new T[capacity];
            if (_size > 0)
            {
                // memory copying of arrays
                var byteCount = UnsafeSize<T>.value * _size;
                ref var source = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(_items)!);
                ref var dest = ref Unsafe.As<T, byte>(ref MemoryMarshal.GetArrayDataReference(newItems)!);
                Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)byteCount);
            }

            _items = newItems;
        }

        #region "nest class"

        private struct Enumerator : IEnumerator<T>
        {
            private readonly FastList<T> _list;
            private int _index;
            private readonly int _version;
            [AllowNull, MaybeNull] private T _current;

            public Enumerator(FastList<T> list)
            {
                _list = list;
                _index = 0;
                _version = list._version;
                _current = default;
            }

            public T Current => _current!;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 && _index == _list._size + 1)
                        ErrorHelper.ThrowInvalidOperationExceptionIfEnumerationFailed();

                    return this.Current;
                }
            }

            public void Dispose()
            {}

            public bool MoveNext()
            {
                if (_list._version == _version && (uint)_index < (uint)_list._size)
                {
                    _current = _list._items[_index++];
                    return true;
                }
                else
                {
                    this.ValidateEnumeration();
                    _index = _list._size + 1;
                    _current = default;
                    return false;
                }
            }

            public void Reset()
            {
                this.ValidateEnumeration();
                _index = 0;
                _current = default;
            }

            private void ValidateEnumeration()
            {
                if (_list._version != _version)
                {
                    ErrorHelper.ThrowInvalidOperationExceptionIfEnumerationFailed();
                }
            }
        }

        private class ListCopy
        {
            public T[] _items;
            public int _size;
            public int _version;
        }

        private class UnsafeSize<T2>
        {
            public static readonly int value;

            static UnsafeSize()
            {
                value = Unsafe.SizeOf<T2>();
            }
        }

        #endregion

    }

}