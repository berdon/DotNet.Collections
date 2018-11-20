using System;
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
    }
}
