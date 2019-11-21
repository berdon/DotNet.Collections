using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DotNet.Collections.Tests
{
    public class OrderedSetTests
    {
        [Fact]
        public void OrderedSet_Implements_ISet()
        {
            Assert.True(new OrderedSet<string>() is ISet<string>);
        }

        [Fact]
        public void OrderedSet_IsEmptyWhenConstructed()
        {
            var set = new OrderedSet<string>();
            Assert.Empty(set);
        }

        [Fact]
        public void OrderedSet_Supports_AddingValues()
        {
            var set = new OrderedSet<string>();
            set.Add("foo");
            Assert.Single(set);
        }

        [Fact]
        public void OrderedSet_Add_ReturnsTrueIfItemDoesNotExist()
        {
            var set = new OrderedSet<string>();
            Assert.True(set.Add("foo"));
        }

        [Fact]
        public void OrderedSet_Add_ReturnsFalseIfItemExists()
        {
            const string item = "foo";

            var set = new OrderedSet<string>();
            set.Add(item);

            Assert.False(set.Add(item));
        }

        [Fact]
        public void OrderedSet_Contains_ReturnsTrueIfItemExists()
        {
            const string item = "foo";

            var set = new OrderedSet<string>();
            set.Add(item);

            Assert.Contains(item, set);
        }

        [Fact]
        public void OrderedSet_Contains_ReturnsFalseIfItemDoesNotExist()
        {
            const string item = "foo";

            var set = new OrderedSet<string>();

            Assert.DoesNotContain(item, set);
        }

        [Fact]
        public void OrderedSet_OrdersItems_ByInsertionOrder()
        {
            const string firstItem = "foo";
            const string secondItem = "bar";

            var set = new OrderedSet<string>();
            set.Add(firstItem);
            set.Add(secondItem);

            var expectedOrder = new[] { firstItem, secondItem };
            Assert.Equal(expectedOrder, set);
        }

        [Fact]
        public void OrderedSet_EnumeratesItems_ByInsertionOrder()
        {
            const string firstItem = "foo";
            const string secondItem = "foo";

            var set = new OrderedSet<string>();
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
        public void OrderedSet_CopiesTo_ByInsertionOrder()
        {
            IEnumerable<string> expectedItems = Enumerable.Range(0, 10).Reverse().Select(v => v.ToString());

            var set = new OrderedSet<string>();
            foreach (var item in expectedItems)
            {
                set.Add(item);
            }

            var copiedArray = new string[10];
            set.CopyTo(copiedArray, 0);

            Assert.Equal(expectedItems, copiedArray);
        }

        [Fact]
        public void OrderedSet_CopiesTo_ByInsertionOrder_AndRespectsArrayIndex()
        {
            IEnumerable<string> expectedItems = Enumerable.Range(0, 10).Reverse().Select(v => v.ToString());

            var set = new OrderedSet<string>();
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
