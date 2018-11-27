using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DotNet.Collections.Tests
{
    public class ConcurrentOrderedSetTests
    {
        [Fact]
        public void ConcurrentOrderedSet_IsEmptyWhenConstructed()
        {
            var set = new ConcurrentOrderedSet<string>();
            Assert.Empty(set);
        }

        [Fact]
        public void ConcurrentOrderedSet_Supports_AddingValues()
        {
            var set = new ConcurrentOrderedSet<string>();
            set.Add("foo");
            Assert.Single(set);
        }

        [Fact]
        public void ConcurrentOrderedSet_Add_ReturnsTrueIfItemDoesNotExist()
        {
            var set = new ConcurrentOrderedSet<string>();
            Assert.True(set.Add("foo"));
        }

        [Fact]
        public void ConcurrentOrderedSet_Add_ReturnsFalseIfItemExists()
        {
            const string item = "foo";

            var set = new ConcurrentOrderedSet<string>();
            set.Add(item);

            Assert.False(set.Add(item));
        }

        [Fact]
        public void ConcurrentOrderedSet_Contains_ReturnsTrueIfItemExists()
        {
            const string item = "foo";

            var set = new ConcurrentOrderedSet<string>();
            set.Add(item);

            Assert.Contains(item, set);
        }

        [Fact]
        public void ConcurrentOrderedSet_Contains_ReturnsFalseIfItemDoesNotExist()
        {
            const string item = "foo";

            var set = new ConcurrentOrderedSet<string>();

            Assert.DoesNotContain(item, set);
        }

        [Fact]
        public void ConcurrentOrderedSet_OrdersItems_ByInsertionOrder()
        {
            const string firstItem = "foo";
            const string secondItem = "bar";

            var set = new ConcurrentOrderedSet<string>();
            set.Add(firstItem);
            set.Add(secondItem);

            var expectedOrder = new[] { firstItem, secondItem };
            Assert.Equal(expectedOrder, set);
        }

        [Fact]
        public void ConcurrentOrderedSet_EnumeratesItems_ByInsertionOrder()
        {
            const string firstItem = "foo";
            const string secondItem = "foo";

            var set = new ConcurrentOrderedSet<string>();
            set.Add(firstItem);
            set.Add(secondItem);

            using (IEnumerator<string> enumerator = set.GetEnumerator())
            {
                enumerator.MoveNext();
                Assert.Equal(firstItem, enumerator.Current);
                enumerator.MoveNext();
                Assert.Equal(secondItem, enumerator.Current);
            }
        }

        [Fact]
        public void ConcurrentOrderedSet_CopiesTo_ByInsertionOrder()
        {
            IEnumerable<string> expectedItems = Enumerable.Range(0, 10).Reverse().Select(v => v.ToString());

            var set = new ConcurrentOrderedSet<string>();
            foreach (var item in expectedItems)
            {
                set.Add(item);
            }

            var copiedArray = new string[10];
            set.CopyTo(copiedArray, 0);

            Assert.Equal(expectedItems, copiedArray);
        }

        [Fact]
        public void ConcurrentOrderedSet_CopiesTo_ByInsertionOrder_AndRespectsArrayIndex()
        {
            IEnumerable<string> expectedItems = Enumerable.Range(0, 10).Reverse().Select(v => v.ToString());

            var set = new ConcurrentOrderedSet<string>();
            foreach (var item in expectedItems)
            {
                set.Add(item);
            }

            var copiedArray = new string[15];
            set.CopyTo(copiedArray, 5);

            Assert.Equal(new string[5], copiedArray.Take(5));
            Assert.Equal(expectedItems, copiedArray.Skip(5));
        }
    }
}
