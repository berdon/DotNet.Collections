using System;
using Xunit;

namespace DotNet.Collections.Tests
{
    public class OrderedSetTests
    {
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
    }
}
