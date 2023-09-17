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

Editting...

## License

ZuList is licensed under the MIT License.  
Some source code borrows [dotnet/runtime](https://github.com/dotnet/runtime)
