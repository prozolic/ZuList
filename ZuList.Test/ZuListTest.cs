namespace ZuList.Test
{
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
                    Assert.Equal(8, list.Capacity);
                    Assert.True(array.SequenceEqual(list));
                }

                [Fact]
                public void RefValueIEnumerable()
                {
                    var list = new FastList<string>();
                    list.AddRange((IEnumerable<string>)strArray);

                    Assert.Equal(strArray.Length, list.Count);
                    Assert.Equal(8, list.Capacity);
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
                    Assert.False(list.Contains(1));
                }

                [Fact]
                public void NonValueRef()
                {
                    var list = new FastList<string>();
                    Assert.False(list.Contains(""));
                    Assert.False(list.Contains("test"));
                }

                [Fact]
                public void Value()
                {
                    var list = new FastList<int>(new[] { 1, 2, 3 });
                    Assert.True(list.Contains(1));
                    Assert.True(list.Contains(2));
                    Assert.True(list.Contains(3));
                    Assert.False(list.Contains(4));
                }

                [Fact]
                public void ValueRef()
                {
                    var list = new FastList<string>(new[] { "test", "test2", "test3" });
                    Assert.True(list.Contains("test"));
                    Assert.True(list.Contains("test2"));
                    Assert.True(list.Contains("test3"));
                    Assert.False(list.Contains("test4"));
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
                    var list = new FastList<int>(new[] {1,2,3});
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
                    var list = new FastList<string>(new[] {"", "test", "test2"});
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