namespace ZuList.Test
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using ZuList;

    public class ZuListTest
    {
        private class TestClass
        {
            public string Name = string.Empty;
            public int Id;
        }

        public class Constructor
        {
            [Fact]
            public void NonArgs()
            {
                var list = new FastList<int>();
            }

            [Fact]
            public void CapacityArgs()
            {
                var list = new FastList<int>(0);
                Assert.Equal(0, list.Capacity);

                var list2 = new FastList<int>(4);
                Assert.Equal(4, list2.Capacity);

                var list3 = new FastList<int>(6);
                Assert.Equal(6, list3.Capacity);
            }

            [Fact]
            public void Args()
            {
                {
                    var fastList = new ZuList.FastList<int>();
                    fastList.Add(2);
                    var defList = new List<int>();
                    defList.Add(2);
                    {
                        // FastList
                        var fastList2 = new ZuList.FastList<int>(fastList);
                        Assert.Equal(fastList.Count, fastList2.Count);
                        Assert.Equal(fastList.Count, fastList2.Capacity);
                        Assert.Equal(fastList[0], fastList2[0]);
                    }
                    {
                        // List(ICollection)
                        var fastList2 = new ZuList.FastList<int>(defList);
                        Assert.Equal(defList.Count, fastList2.Count);
                        Assert.Equal(defList.Count, fastList2.Capacity);
                        Assert.Equal(defList[0], fastList2[0]);
                    }
                }
                {
                    var fastList = new List<TestClass>();
                    fastList.Add(new TestClass() { Name = "TEST", Id = 1 });
                    fastList.Add(new TestClass() { Name = "TEST2", Id = 2 });
                    var defList = new ZuList.FastList<TestClass>();
                    defList.Add(new TestClass() { Name = "TEST", Id = 1 });
                    defList.Add(new TestClass() { Name = "TEST2", Id = 2 });
                    {
                        // FastList
                        var fastList2 = new ZuList.FastList<TestClass>(fastList);
                        Assert.Equal(fastList.Count, fastList2.Count);
                        Assert.Equal(fastList.Count, fastList2.Capacity);
                        Assert.Equal(fastList[0], fastList2[0]);
                        Assert.Equal(fastList[1], fastList2[1]);
                    }
                    {
                        // List(ICollection)
                        var fastList2 = new ZuList.FastList<TestClass>(defList);
                        Assert.Equal(defList.Count, fastList2.Count);
                        Assert.Equal(defList.Count, fastList2.Capacity);
                        Assert.Equal(defList[0], fastList2[0]);
                        Assert.Equal(defList[1], fastList2[1]);
                    }

                }
            }
        }

        public class ForEach
        {
            [Fact]
            public void Validate()
            {
                var foreachInvoker = () =>
                {
                    try
                    {
                        var list = new FastList<string>(new[] { "1", "2", "3", "4", "5" });
                        foreach (var item in list)
                        { }
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                };
                Assert.True(foreachInvoker());
            }
        }

        public class ListCompatible
        {
            public class AddRange
            {
                private static readonly int[] array = Enumerable.Range(1, 5).ToArray();
                private static readonly string[] strArray = array.Select(a => a.ToString()).ToArray();

                [Fact]
                public void ValueIEnumerable()
                {
                    var list = new FastList<int>();
                    list.AddRange((IEnumerable<int>)array);

                    Assert.Equal(array.Length, list.Count);
                    Assert.Equal(list.Count, list.Capacity);
                    Assert.True(array.SequenceEqual(list));
                }

                [Fact]
                public void RefValueIEnumerable()
                {
                    var list = new FastList<string>();
                    list.AddRange((IEnumerable<string>)strArray);

                    Assert.Equal(strArray.Length, list.Count);
                    Assert.Equal(list.Count, list.Capacity);
                    Assert.True(strArray.SequenceEqual(list));
                }

                [Fact]
                public void ValueArray()
                {
                    var list = new FastList<int>();
                    list.AddRange(array);

                    Assert.Equal(array.Length, list.Count);
                    Assert.Equal(array.Length, list.Capacity);
                    Assert.True(array.SequenceEqual(list));
                }

                [Fact]
                public void RefValueArray()
                {
                    var list = new FastList<string>();
                    list.AddRange(strArray);

                    Assert.Equal(strArray.Length, list.Count);
                    Assert.Equal(strArray.Length, list.Capacity);
                    Assert.True(strArray.SequenceEqual(list));
                }
            }

            public class AsReadOnly
            {
                [Fact]
                public void Validate()
                {
                    var list = new FastList<int>(Enumerable.Range(1, 5).ToArray());
                    var readonlycollection = list.AsReadOnly();

                    Assert.True(readonlycollection.GetType() == typeof(ReadOnlyCollection<int>));
                }
            }

            public class BinarySearch
            {
                private static readonly int[] array = Enumerable.Range(1, 5).ToArray();
                private static readonly string[] strArray = array.Select(a => a.ToString()).ToArray();

                [Fact]
                public void Validate()
                {
                    var list = new FastList<int>();
                    list.AddRange(array);

                    Assert.Equal(1, list.BinarySearch(2));
                    Assert.Equal(4, list.BinarySearch(5));
                }

                [Fact]
                public void ValidateRefValue()
                {
                    var list = new FastList<string>();
                    list.AddRange(strArray);

                    Assert.Equal(1, list.BinarySearch("2"));
                    Assert.Equal(4, list.BinarySearch("5"));
                }
            }

            public class ConvertAll
            {
                [Fact]
                public void Validate()
                {
                    var rangeCount = 5;
                    var intRangeArr = Enumerable.Range(1, rangeCount).ToArray();
                    var stringRangeArr = new[] { "1", "2", "3", "4", "5" };
                    var list = new FastList<int>(Enumerable.Range(1, rangeCount).ToArray());
                    var stringList = list.ConvertAll(x => x.ToString());

                    Assert.Equal(rangeCount, stringList.Count);
                    Assert.Equal(rangeCount, stringList.Capacity);
                    Assert.True(stringRangeArr.SequenceEqual(stringList));
                }

                [Fact]
                public void ThrowArgumentNull()
                {
                    var list = new FastList<int>();
                    Assert.Throws<ArgumentNullException>(() => list.ConvertAll<string>(null));
                }
            }

            public class EnsureCapacity
            {
                [Fact]
                public void Validate()
                {
                    var rangeCount = 5;
                    var list = new FastList<int>(Enumerable.Range(1, rangeCount).ToArray());

                    {
                        var currentCapacity = list.EnsureCapacity(3);
                        Assert.Equal(rangeCount, currentCapacity);
                        Assert.Equal(rangeCount, list.Count);
                        Assert.Equal(rangeCount, list.Capacity);
                    }
                    {
                        var ensureCapacityValue = 10;
                        var currentCapacity = list.EnsureCapacity(ensureCapacityValue);
                        Assert.Equal(ensureCapacityValue, currentCapacity);
                        Assert.Equal(rangeCount, list.Count);
                        Assert.Equal(ensureCapacityValue, list.Capacity);
                    }
                }
            }

            public class Exists
            {
                [Fact]
                public void Validate()
                {
                    var rangeCount = 5;
                    var list = new FastList<int>(Enumerable.Range(1, rangeCount).ToArray());
                    Assert.True(list.Exists(i => i == 1));
                    Assert.False(list.Exists(i => i > 5));
                }
            }

            public class Find
            {
                [Fact]
                public void Validate()
                {
                    var list = new FastList<string>(new[] { "1", "2", "3", "4", "5" });
                    Assert.Equal("1", list.Find(i => i == "1"));
                    Assert.Null(list.Find(i => i == "Error"));
                }

                [Fact]
                public void ThrowArgumentNull()
                {
                    var list = new FastList<int>();
                    Assert.Throws<ArgumentNullException>(() => list.Find(null));
                }
            }

            public class FindIndex
            {
                [Fact]
                public void Validate()
                {
                    var list = new FastList<string>(new[] { "1", "2", "3", "4", "5" });
                    Assert.Equal(0, list.FindIndex(i => i == "1"));
                    Assert.Equal(-1, list.FindIndex(i => i == "Error"));
                }

                [Fact]
                public void ThrowArgumentNull()
                {
                    var list = new FastList<int>();
                    Assert.Throws<ArgumentNullException>(() => list.FindIndex(null));
                }
            }

            public class FindLast
            {
                [Fact]
                public void Validate()
                {
                    var list = new FastList<string>(new[] { "1", "2", "3", "4", "5" });
                    Assert.Equal("1", list.FindLast(i => i == "1"));
                    Assert.Null(list.FindLast(i => i == "Error"));
                }

                [Fact]
                public void ThrowArgumentNull()
                {
                    var list = new FastList<int>();
                    Assert.Throws<ArgumentNullException>(() => list.FindLast(null));
                }
            }

            public class FindLastIndex
            {
                [Fact]
                public void Validate()
                {
                    var list = new FastList<string>(new[] { "1", "2", "3", "4", "5" });
                    Assert.Equal(0, list.FindIndex(i => i == "1"));
                    Assert.Equal(-1, list.FindIndex(i => i == "Error"));
                }

                [Fact]
                public void ThrowArgumentNull()
                {
                    var list = new FastList<int>();
                    Assert.Throws<ArgumentNullException>(() => list.FindIndex(null));
                }
            }

            public class FindAll
            {
                [Fact]
                public void Validate()
                {
                    var list = new FastList<int>(Enumerable.Range(1, 5).ToArray());
                    Assert.True(new[] { 1 }.SequenceEqual(list.FindAll(i => i == 1)));
                    Assert.True(new[] { 4, 5 }.SequenceEqual(list.FindAll(i => i > 3)));
                    Assert.True(Array.Empty<int>().SequenceEqual(list.FindAll(i => i > 5)));
                }

                [Fact]
                public void ThrowArgumentNull()
                {
                    var list = new FastList<int>();
                    Assert.Throws<ArgumentNullException>(() => list.FindAll(null));
                }
            }

            public class ForEach
            {
                [Fact]
                public void Validate()
                {
                    var list = new FastList<string>(new[] { "1", "2", "3", "4", "5" });
                    var result = new FastList<string>(5);
                    list.ForEach(i => result.Add(i));
                    Assert.True(list.SequenceEqual(result));
                }
            }

            public class InsertRange
            {
                private static readonly int[] array = Enumerable.Range(1, 5).ToArray();
                private static readonly string[] strArray = array.Select(a => a.ToString()).ToArray();

                [Fact]
                public void ValidateValue()
                {
                    var list = new FastList<int>();
                    list.InsertRange(0,(IEnumerable<int>)array);

                    Assert.Equal(array.Length, list.Count);
                    Assert.Equal(8, list.Capacity);
                    Assert.True(array.SequenceEqual(list));

                    list.InsertRange(2, array);

                    Assert.Equal(array.Length * 2, list.Count);
                    Assert.Equal(16, list.Capacity);
                    Assert.True(new[] {1,2,1,2,3,4,5,3,4,5}.SequenceEqual(list));
                }

                [Fact]
                public void ValidateRefValue()
                {
                    var list = new FastList<string>();
                    list.InsertRange(0, (IEnumerable<string>)strArray);

                    Assert.Equal(strArray.Length, list.Count);
                    Assert.Equal(8, list.Capacity);
                    Assert.True(strArray.SequenceEqual(list));

                    list.InsertRange(2, strArray);

                    Assert.Equal(array.Length * 2, list.Count);
                    Assert.Equal(16, list.Capacity);
                    Assert.True(new[] { "1", "2", "1", "2", "3", "4", "5", "3", "4", "5" }.SequenceEqual(list));
                }
            }
        }

        public class IListGeneric
        {
            public class Add
            {
                [Fact]
                public void Value()
                {
                    var addValue = 1;
                    var list = new FastList<int>();
                    list.Add(addValue);
                    Assert.Equal(1, list.Count);
                    Assert.Equal(4, list.Capacity);
                    Assert.Equal(addValue, list[0]);
                }

                [Fact]
                public void ValueRef()
                {
                    var addValue = "test";
                    var list = new FastList<string>();
                    list.Add(addValue);
                    Assert.Equal(1, list.Count);
                    Assert.Equal(4, list.Capacity);
                    Assert.Equal(addValue, list[0]);
                }
            }

            public class Clear
            {
                [Fact]
                public void NonValue()
                {
                    var list = new FastList<int>();
                    list.Clear();
                    Assert.Equal(0, list.Count);
                    Assert.Equal(0, list.Capacity);
                }

                [Fact]
                public void NonValue2()
                {
                    var list = new FastList<string>();
                    list.Clear();
                    Assert.Equal(0, list.Count);
                    Assert.Equal(0, list.Capacity);
                }

                [Fact]
                public void Value()
                {
                    var addValue = 1;
                    var list = new FastList<int>();
                    list.Add(addValue);
                    list.Clear();
                    Assert.Equal(0, list.Count);
                    Assert.Equal(4, list.Capacity);
                }

                [Fact]
                public void Value2()
                {
                    var addValue = "test";
                    var list = new FastList<string>();
                    list.Add(addValue);
                    list.Clear();
                    Assert.Equal(0, list.Count);
                    Assert.Equal(4, list.Capacity);
                }
            }

            public class Contains
            {
                [Fact]
                public void NonValue()
                {
                    var list = new FastList<int>();
                    Assert.DoesNotContain(1, list);
                }

                [Fact]
                public void NonValueRef()
                {
                    var list = new FastList<string>();
                    Assert.DoesNotContain("", list);
                    Assert.DoesNotContain("test", list);
                }

                [Fact]
                public void Value()
                {
                    var list = new FastList<int>(new[] { 1, 2, 3 });
                    Assert.Contains(1, list);
                    Assert.Contains(2, list);
                    Assert.Contains(3, list);
                    Assert.DoesNotContain(4, list);
                }

                [Fact]
                public void ValueRef()
                {
                    var list = new FastList<string>(new[] { "test", "test2", "test3" });
                    Assert.Contains("test", list);
                    Assert.Contains("test2", list);
                    Assert.Contains("test3", list);
                    Assert.DoesNotContain("test4", list);
                }
            }

            public class Copyto
            {

            }

            public class IndexOf
            {
                [Fact]
                public void NonValue()
                {
                    var list = new FastList<int>();
                    Assert.True(list.IndexOf(1) < 0);
                }

                [Fact]
                public void NonValueRef()
                {
                    var list = new FastList<string>();
                    Assert.True(list.IndexOf("") < 0);
                    Assert.True(list.IndexOf("test") < 0);
                }

                [Fact]
                public void Value()
                {
                    var list = new FastList<int>(new[] { 1, 2, 3 });
                    Assert.True(list.IndexOf(1) == 0);
                    Assert.True(list.IndexOf(2) == 1);
                    Assert.True(list.IndexOf(3) == 2);
                    Assert.True(list.IndexOf(4) < 0);
                }

                [Fact]
                public void ValueRef()
                {
                    var list = new FastList<string>(new[] { "test", "test2", "test3" });
                    Assert.True(list.IndexOf("test") == 0);
                    Assert.True(list.IndexOf("test2") == 1);
                    Assert.True(list.IndexOf("test3") == 2);
                    Assert.True(list.IndexOf("test4") < 0);
                }
            }

            public class Insert
            {
                [Fact]
                public void Value()
                {
                    var list = new FastList<int>();
                    list.Insert(0, 1);
                    Assert.Equal(1, list.Count);
                    Assert.Equal(4, list.Capacity);
                    Assert.Equal(1, list[0]);

                    list.Insert(0, -1);
                    Assert.Equal(2, list.Count);
                    Assert.Equal(4, list.Capacity);
                    Assert.Equal(-1, list[0]);
                }

                [Fact]
                public void ValueRef()
                {
                    var list = new FastList<string>();
                    list.Insert(0, "test");
                    Assert.Equal(1, list.Count);
                    Assert.Equal(4, list.Capacity);
                    Assert.Equal("test", list[0]);

                    list.Insert(0, "test2");
                    Assert.Equal(2, list.Count);
                    Assert.Equal(4, list.Capacity);
                    Assert.Equal("test2", list[0]);
                }

                [Fact]
                public void ThrowArgumentOutOfRangeException()
                {
                    var addValue = 1;
                    var list = new FastList<int>();
                    list.Add(addValue);
                    Assert.Throws<ArgumentOutOfRangeException>(() => { list.Insert(list.Count + 1, 2048); });
                }

            }

            public class Remove
            {
                [Fact]
                public void NonValue()
                {
                    var list = new FastList<int>();
                    Assert.False(list.Remove(0));
                    Assert.False(list.Remove(1));
                    Assert.Equal(0, list.Count);
                    Assert.Equal(0, list.Capacity);
                }

                [Fact]
                public void NonValue2()
                {
                    var list = new FastList<string>();
                    Assert.False(list.Remove(""));
                    Assert.False(list.Remove("test"));
                    Assert.Equal(0, list.Count);
                    Assert.Equal(0, list.Capacity);
                }

                [Fact]
                public void Value()
                {
                    var list = new FastList<int>(new[] { 1, 2, 3 });
                    Assert.False(list.Remove(4));
                    Assert.Equal(3, list.Count);

                    Assert.True(list.Remove(1));
                    Assert.Equal(2, list.Count);

                    Assert.True(list.Remove(2));
                    Assert.Equal(1, list.Count);

                    Assert.True(list.Remove(3));
                    Assert.Equal(0, list.Count);
                }

                [Fact]
                public void Value2()
                {
                    var list = new FastList<string>(new[] { "", "test", "test2" });
                    Assert.False(list.Remove("NoValue"));
                    Assert.Equal(3, list.Count);

                    Assert.True(list.Remove(""));
                    Assert.Equal(2, list.Count);

                    Assert.True(list.Remove("test"));
                    Assert.Equal(1, list.Count);

                    Assert.True(list.Remove("test2"));
                    Assert.Equal(0, list.Count);
                }
            }

            public class RemoveAt
            {
                [Fact]
                public void NonValue()
                {
                    var list = new FastList<int>();
                    Assert.Throws<ArgumentOutOfRangeException>(() => { list.RemoveAt(0); });
                    Assert.Equal(0, list.Count);
                    Assert.Equal(0, list.Capacity);
                }

                [Fact]
                public void NonValue2()
                {
                    var list = new FastList<string>();
                    Assert.Throws<ArgumentOutOfRangeException>(() => { list.RemoveAt(0); });
                    Assert.Equal(0, list.Count);
                    Assert.Equal(0, list.Capacity);
                }

                [Fact]
                public void Value()
                {
                    var list = new FastList<int>(new[] { 1, 2, 3 });
                    Assert.Throws<ArgumentOutOfRangeException>(() => { list.RemoveAt(list.Count); });
                    Assert.Equal(3, list.Count);

                    list.RemoveAt(2);
                    Assert.Equal(2, list.Count);

                    list.RemoveAt(1);
                    Assert.Equal(1, list.Count);

                    list.RemoveAt(0);
                    Assert.Equal(0, list.Count);
                }

                [Fact]
                public void Value2()
                {
                    var list = new FastList<string>(new[] { "", "test", "test2" });
                    Assert.Throws<ArgumentOutOfRangeException>(() => { list.RemoveAt(list.Count); });
                    Assert.Equal(3, list.Count);

                    list.RemoveAt(2);
                    Assert.Equal(2, list.Count);

                    list.RemoveAt(1);
                    Assert.Equal(1, list.Count);

                    list.RemoveAt(0);
                    Assert.Equal(0, list.Count);
                }
            }
        }
    }
}