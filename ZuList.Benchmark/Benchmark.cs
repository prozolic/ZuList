using BenchmarkDotNet.Attributes;
using System;

namespace ZuList.Benchmark
{
    using ZuList;

    //[ShortRunJob]
    [MemoryDiagnoser]
    [RankColumn]
    public class Benchmark
    {

    }
}
