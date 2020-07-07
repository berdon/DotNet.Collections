using System;
using System.Linq;
using Moq;
using Xunit;

namespace DotNet.Collections.Tests
{
    public interface IValueGenerator<TValue>
    {
        TValue ValueGenerator();
    }

    public class LruCacheTests
    {
        [Fact]
        public void LruCache_Supports_ZeroCapacity()
        {
            var cache = new LruCache<string, string>(0);
            cache.Add("foo", "bar");
            Assert.Equal(0, cache.Count);
        }

        [Fact]
        public void LruCache_Supports_AddingValue()
        {
            var cache = new LruCache<string, string>(100);
            cache.Add("foo", "bar");
        }

        [Fact]
        public void LruCache_AddingValue_OverCapacity_RemovesOldest()
        {
            var cache = new LruCache<string, string>(1);
            cache.Add("foo", "bar");
            cache.Add("bar", "foo");

            Assert.False(cache.TryGetValue("foo", out var foo));
            Assert.True(cache.TryGetValue("bar", out var bar));
        }

        [Fact]
        public void LruCache_AddingValue_ThatAlreadyExists_Replaces()
        {
            const string key = "foo";
            const string expectedValue = "foo";

            var cache = new LruCache<string, string>(1);
            cache.Add(key, "bar");
            cache.Add(key, expectedValue);

            cache.TryGetValue(key, out var value);
            Assert.Equal(expectedValue, value);
        }

        [Fact]
        public void LruCache_AddingValue_ThatAlreadyExists_RenewsRecentlyUsedStatus()
        {
            const string key = "foo";
            const string expiredKey = "bar";

            var cache = new LruCache<string, string>(2);
            cache.Add(key, "bar");
            cache.Add(expiredKey, "foo");
            cache.Add(key, "foo");

            // Adding this next item should expire "bar", not "foo"
            cache.Add("another", "another");

            Assert.True(cache.TryGetValue(key, out var value));
            Assert.False(cache.TryGetValue(expiredKey, out var val));
        }

        [Fact]
        public void LruCache_Supports_GettingSpecifiedCapacity()
        {
            const int expectedCapacity = 100;

            var cache = new LruCache<string, string>(expectedCapacity);
            Assert.Equal(expectedCapacity, cache.Capacity);
        }

        [Fact]
        public void LruCache_Supports_GettingItemCount()
        {
            const int expectedCount = 50;

            var cache = new LruCache<string, string>(100);
            foreach (var value in Enumerable.Range(0, expectedCount).Select(v => v.ToString()))
            {
                cache.Add(value, value);
            }
            Assert.Equal(expectedCount, cache.Count);
        }

        [Fact]
        public void LruCache_TryGetValue_ReturnsValueIfExists()
        {
            const string key = "foo";
            const string expectedValue = "bar";

            var cache = new LruCache<string, string>(100);
            cache.Add(key, expectedValue);
            cache.TryGetValue(key, out var value);
            Assert.Equal(expectedValue, value);
        }

        [Fact]
        public void LruCache_TryGetValue_ReturnsTrueIfValueExists()
        {
            const string key = "foo";
            const string expectedValue = "bar";

            var cache = new LruCache<string, string>(100);
            cache.Add(key, expectedValue);
            Assert.True(cache.TryGetValue(key, out var value));
        }

        [Fact]
        public void LruCache_TryGetValue_ReturnsFalseIfValueDoesNotExist()
        {
            var cache = new LruCache<string, string>(100);
            Assert.False(cache.TryGetValue("does not exist", out var _));
        }

        [Fact]
        public void LruCache_TryGetValue_ReturnsFalseIfKeyRemoved()
        {
            const string key = "foo";
            const string expectedValue = "bar";

            var cache = new LruCache<string, string>(100);

            cache.Add(key, expectedValue);
            Assert.True(cache.TryGetValue(key, out var value));

            cache.Remove(key);
            Assert.False(cache.TryGetValue("does not exist", out var _));
        }

        [Fact]
        public void LruCache_TryGetValue_OutReturnsTypeDefaultIfNotExists()
        {
            var cache = new LruCache<string, string>(100);
            cache.TryGetValue("does not exist", out var value);

            Assert.Equal(default(string), value);
        }

        [Fact]
        public void LruCache_TryGetValue_UpdatesExpirationOfKey()
        {
            const string key = "foo";
            const string expiredKey = "bar";

            var cache = new LruCache<string, string>(2);
            cache.Add(key, "foo");
            cache.Add(expiredKey, "bar");

            cache.TryGetValue(key, out var value);

            cache.Add("another", "another");

            Assert.True(cache.TryGetValue(key, out var first));
            Assert.False(cache.TryGetValue(expiredKey, out var second));
        }

        [Fact]
        public void LruCache_Supports_GettingValue()
        {
            const string key = "foo";
            const string expectedValue = "bar";

            var cache = new LruCache<string, string>(100);
            cache.Add(key, expectedValue);
            Assert.Equal(expectedValue, cache.Get(key, () => null));
        }

        [Fact]
        public void LruCache_CallsValueGenerator_IfKeyDoesNotExist()
        {
            var cache = new LruCache<string, string>(100);

            var valueGeneratorMock = new Mock<IValueGenerator<string>>();
            cache.Get("foo", valueGeneratorMock.Object.ValueGenerator);

            valueGeneratorMock.Verify(m => m.ValueGenerator(), Times.Once);
        }

        [Fact]
        public void LruCache_Get_UpdatesExpirationOfKey()
        {
            const string key = "foo";
            const string expiredKey = "bar";

            var cache = new LruCache<string, string>(2);
            cache.Add(key, "foo");
            cache.Add(expiredKey, "bar");

            var valueGeneratorMock = new Mock<IValueGenerator<string>>();
            cache.Get(key, valueGeneratorMock.Object.ValueGenerator);
            valueGeneratorMock.Verify(m => m.ValueGenerator(), Times.Never);

            cache.Add("another", "another");

            Assert.True(cache.TryGetValue(key, out var first));
            Assert.False(cache.TryGetValue(expiredKey, out var second));
        }

        [Fact]
        public void LruCache_Clear_EmptiesCollection()
        {
            const int expectedCount = 50;

            var cache = new LruCache<string, string>(100);
            foreach (var value in Enumerable.Range(0, expectedCount).Select(v => v.ToString()))
            {
                cache.Add(value, value);
            }
            Assert.Equal(expectedCount, cache.Count);

            cache.Clear();

            Assert.Equal(0, cache.Count);
        }
    }
}
