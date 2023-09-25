# ZuList

ZuList is collection that runs faster than `System.Collections.Generic.List`.  
We target .NET 7 and The library code is pure C#.  

* It can be used in the same way as `System.Collections.Generic.List<T>`
* Internal use of `Span` for faster speed

## Quick Start

It can be used in the same way as `System.Collections.Generic.List<T>`.

```csharp
public void Sample()
{
    var list = new FastList<int>();
    list.Add(1);
    list.Add(2);
    list.Add(3);

    Console.WriteLine($@"list.Count = {list.Count}, {list.ToDebugString()}"); // list.Count = 3, [ 1, 2, 3 ]
}
```

## API Reference

**class FastList**

| method | description |
| -- | -- |
| `public void AddRange(FastList<T> fastList)` | Adds the elements of the specified `FastList<T>` to the end of the `FastList<T>`. |
| `public void AddRange(T[] array)` | Adds the elements of the specified array to the end of the `FastList<T>`. |
| `public void AddRange(ICollection<T> collection)` | Adds the elements of the specified `ICollection<T>` to the end of the `FastList<T>`. |
| `public void InsertRange(int index, FastList<T> fastList)`  | Inserts an element of the `FastList<T>` at the specified index position in the `FastList<T>`. |
| `public void InsertRange(int index, T[] array)`  | Inserts an element of the array at the specified index position in the `FastList<T>`. |
| `public void InsertRange(int index, ICollection<T> collection)`  | Inserts an element of the `ICollection<T>` at the specified index position in the `FastList<T>`. |
| `public List<T> ToList()`  | Copies the elements of `FastList<T>` to a new `System.Collections.Generic.List<T>`. |
| `public string ToDebugString()`  | `FastList<T>` Returns a debugging string representing |

| property | description |
| -- | -- |
| `public bool IsEmpty` |  |

**Compatibility with `List<T>`**

| method | description |
| -- | -- |
| `public void AddRange(IEnumerable<T> collection)` |  |
| `public ReadOnlyCollection<T> AsReadOnly()` |  |
| `public int BinarySearch(T item)` |  |
| `public int BinarySearch(T item, IComparer<T>? comparer)` |  |
| `public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)` |  |
| `public FastList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)` |  |
| `public int EnsureCapacity(int capacity)`  |  |
| `public bool Exists(Predicate<T> match)`  |  |
| `public T? Find(Predicate<T> match)`  |  |
| `public int FindIndex(Predicate<T> match)`  |  |
| `public int FindIndex(int startIndex, Predicate<T> match)`  |  |
| `public int FindIndex(int startIndex, int count, Predicate<T> match)`  |  |
| `public T? FindLast(Predicate<T> match)`  |  |
| `public int FindLastIndex(Predicate<T> match)`  |  |
| `public int FindLastIndex(int startIndex, Predicate<T> match)`  |  |
| `public int FindLastIndex(int startIndex, int count, Predicate<T> match)`  |  |
| `public FastList<T> FindAll(Predicate<T> match)`  |  |
| `public void ForEach(Action<T> action)`  |  |
| `public FastList<T> GetRange(int index, int count)`  |  |
| `public void InsertRange(int index, IEnumerable<T> collection)`  |  |
| `public int RemoveAll(Predicate<T> match)`  |  |
| `public void RemoveRange(int index, int count)`  |  |
| `public void Reverse()`  |  |
| `public void Reverse(int index, int count)`  |  |
| `public int ShrinkToFitCapacity()`  |  |
| `public FastList<T> Slice(int start, int length)`  |  
| `public void Sort()`  |  |
| `public void Sort(IComparer<T>? comparer)`  |  |
| `public void Sort(int index, int count, IComparer<T>? comparer)`  |  |
| `public void Sort(Comparison<T> comparison)`  |  |
| `public T[] ToArray()`  |  |
| `public void TrimExcess()`  |  |
| `public bool TrueForAll(Predicate<T> match)`  |  |

| property | description |  
| -- | -- |  
| `public int Capacity`  |  |  

**class FastList : `IList<T>`**

| method | description |
| -- | -- |
| `public void Add(T item)` |  |
| `public void Clear()` |  |
| `public bool Contains(T item)` |  |
| `public void CopyTo(T[] array, int arrayIndex)` |  |
| `public IEnumerator<T> GetEnumerator()` |  |
| `public int IndexOf(T item)` |  |
| `public void Insert(int index, T item)` |  |
| `public bool Remove(T item)` |  |
| `public void RemoveAt(int index)` |  |

| property | description |
| -- | -- |
| `public T this[int index]`  |  |
| `public int Count`  |  |
| `public bool IsReadOnly`  |  |

**class FastList : IList**

| method | description |
| -- | -- |
| `public int Add(object? value)` |  |
| `public bool Contains(object? value)` |  |
| `public int IndexOf(object? value)` |  |
| `public void Insert(int index, object? value)` |  |
| `public void Remove(object? value)` |  |
| `public void CopyTo(Array array, int arrayIndex)` |  |

| property | description |
| -- | -- |
| `object? IList.this[int index]`  |  |
| `public bool IsFixedSize`  |  |
| `public bool IsSynchronized`  |  |
| `public object SyncRoot`  |  |

## License

ZuList is licensed under the MIT License.  
Some source code borrows [dotnet/runtime](https://github.com/dotnet/runtime)
