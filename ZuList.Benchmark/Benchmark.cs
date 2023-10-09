using BenchmarkDotNet.Attributes;
using System;

namespace ZuList.Benchmark
{
    using BenchmarkDotNet.Running;
    using ZuList;

    ////[ShortRunJob]
    //[MemoryDiagnoser]
    //[RankColumn]
    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private List<int> testList;
        private List<string> testStrList1000;
        private FastList<int> testFastList;
        private FastList<string> testStrFastList1000;
        private static readonly int[] numArray = Enumerable.Range(1, 100).ToArray();
        private static readonly string[] stringNumArray = numArray.Select(i => i.ToString()).ToArray();

        [GlobalSetup]
        public void GlobalSetup()
        {
            testList = new List<int>(128);
            testStrList1000 = new List<string>(128);
            testFastList = new FastList<int>(128);
            testStrFastList1000 = new FastList<string>(128);

            testList.AddRange(numArray);
            testStrList1000.AddRange(stringNumArray.ToArray());
            testFastList.AddRange(numArray);
            testStrFastList1000.AddRange(stringNumArray.ToArray());
        }

        [BenchmarkCategory("Add"), Benchmark]
        public void Add_List()
        {
            var defList4 = new List<int>();
            for (int index = 0; index < 100; index++)
            {
                defList4.Add(index);
            }
        }

        [BenchmarkCategory("Add"), Benchmark]
        public void Add_FastList()
        {
            var fastList4 = new FastList<int>();
            for (int index = 0; index < 100; index++)
            {
                fastList4.Add(index);
            }
        }

        [BenchmarkCategory("Add"), Benchmark]
        public void Add_List_TestClass()
        {
            var defList4 = new List<TestClass>();
            var test = new TestClass();
            for (int index = 0; index < 100; index++)
            {
                defList4.Add(test);
            }
        }

        [BenchmarkCategory("Add"), Benchmark]
        public void Add_FastList_TestClass()
        {
            var fastList4 = new FastList<TestClass>();
            var test = new TestClass();
            for (int index = 0; index < 100; index++)
            {
                fastList4.Add(test);
            }
        }

        [BenchmarkCategory("AddRange"), Benchmark]
        public void AddRange_List()
        {
            var defList4 = new List<int>();
            for (int index = 0; index < 100; index++)
            {
                defList4.AddRange(numArray);
            }
        }

        [BenchmarkCategory("AddRange"), Benchmark]
        public void AddRange_FastList()
        {
            var fastList4 = new FastList<int>();
            for (int index = 0; index < 100; index++)
            {
                fastList4.AddRange(numArray);
            }
        }

        [BenchmarkCategory("AddRange"), Benchmark]
        public void AddRange_List_ArgsFastList()
        {
            var defList4 = new List<int>();
            for (int index = 0; index < 100; index++)
            {
                defList4.AddRange(testFastList);
            }
        }

        [BenchmarkCategory("AddRange"), Benchmark]
        public void AddRange_FastList_ArgsFastList()
        {
            var fastList4 = new FastList<int>();
            for (int index = 0; index < 100; index++)
            {
                fastList4.AddRange(testFastList);
            }
        }

        [BenchmarkCategory("InsertRange"), Benchmark]
        public void InsertRange_List_int()
        {
            var defList4 = new List<int>();
            var arr = numArray;
            for (int index = 0; index < 100; index++)
            {
                defList4.InsertRange(0, arr);
            }
        }

        [BenchmarkCategory("InsertRange"), Benchmark]
        public void InsertRange_FastList_int()
        {
            var fastList4 = new FastList<int>();
            var arr = numArray;
            for (int index = 0; index < 100; index++)
            {
                fastList4.InsertRange(0, arr);
            }
        }

        [BenchmarkCategory("InsertRange"), Benchmark]
        public void InsertRange_List_string()
        {
            var defList4 = new List<string>();
            var arr = stringNumArray;
            for (int index = 0; index < 100; index++)
            {
                defList4.InsertRange(0, arr);
            }
        }

        [BenchmarkCategory("InsertRange"), Benchmark]
        public void InsertRangeFast_FastList_string()
        {
            var fastList4 = new FastList<string>();
            var arr = stringNumArray;
            for (int index = 0; index < 100; index++)
            {
                fastList4.InsertRange(0, arr);
            }
        }
    }
}
