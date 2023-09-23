# ZuList

ZuList is collection that runs faster than System.Collections.Generic.List.  
We target .NET 7 and The library code is pure C#.  

## Quick Start

It can be used in the same way as System.Collections.Generic.List.

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
| `public void AddRange(FastList<T> fastList)` |  |
| `public void AddRange(T[] array)` |  |
| `public void AddRange(ICollection<T> collection)` | |
| `public void InsertRange(int index, FastList<T> fastList)`  |  |
| `public void InsertRange(int index, T[] array)`  |  |
| `public void InsertRange(int index, ICollection<T> collection)`  |  |
| `public List<T> ToList()`  |  |
| `public string ToDebugString()`  |  |
| `public List<T> ToList()`  |  |

| property | description |
| -- | -- |
| `public bool IsEmpty` |  |

**Compatibility with `List<T>`**

| method | description |
| -- | -- |
| `public void AddRange(IEnumerable<T> collection)` |  |
| `public ReadOnlyCollection<T> AsReadOnly()` |  |
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
| `public T[] ToArray()`  |  |

| property | description | Compatibility with `List<T>` |
| -- | -- | -- |
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

| property | description | Compatibility with `List<T>` |
| -- | -- | -- |
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

| property | description | Compatibility with `List<T>` |
| -- | -- | -- |
| `object? IList.this[int index]`  |  |
| `public bool IsFixedSize`  |  |
| `public bool IsSynchronized`  |  |
| `public object SyncRoot`  |  |

## License

ZuList is licensed under the MIT License.  
Some source code borrows [dotnet/runtime](https://github.com/dotnet/runtime)
