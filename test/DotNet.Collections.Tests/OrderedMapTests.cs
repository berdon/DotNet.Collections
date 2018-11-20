using System;
using System.Collections.Generic;
using Xunit;

namespace DotNet.Collections.Tests
{
    public class OrderedMapTests
    {
        [Fact]
        public void OrderedMap_Supports_ReadingCount()
        {
            var dict = new OrderedDictionary<string, string>();
            Assert.Empty(dict);
        }

        [Fact]
        public void OrderedMap_Supports_Adds()
        {
            var dict = new OrderedDictionary<string, string>();
            dict.Add("foo", "bar");
            Assert.Single(dict);
        }

        [Fact]
        public void OrderedMap_Add_ReturnsTrueIfItemIsAdded()
        {
            var dict = new OrderedDictionary<string, string>();
            Assert.True(dict.Add("foo", "bar"));
        }

        [Fact]
        public void OrderedMap_Add_ReturnsFalseIfKeyExists()
        {
            const string key = "foo";
            const string value = "bar";

            var dict = new OrderedDictionary<string, string>();
            dict.Add(key, value);

            Assert.False(dict.Add(key, value));
        }

        [Fact]
        public void OrderedMap_Supports_AddWithKeyValuePair()
        {
            var dict = new OrderedDictionary<string, string>();
            dict.Add(new KeyValuePair<string, string>("foo", "bar"));
            Assert.Single(dict);
        }

        [Fact]
        public void OrderedMap_Supports_AddOrUpdate()
        {
            var dict = new ConcurrentOrderedDictionary<string, string>();
            var result = dict.AddOrUpdate("foo", "bar", (k, v) => "baz");

            Assert.Single(dict);
            Assert.Equal(result, "bar");

            result = dict.AddOrUpdate("foo", "baz", (k, v) => "baz");

            Assert.Single(dict);
            Assert.Equal(result, "baz");
        }

        [Fact]
        public void OrderedMap_AddOrUpdate_UpdateCallbackNotCalledForNewItem()
        {
            var dict = new ConcurrentOrderedDictionary<string, string>();

            string callbackValue = null;
            dict.AddOrUpdate("foo", "bar", (k, v) => {
                callbackValue = v;
                return "baz";
            });

            Assert.Null(callbackValue);
        }

        [Fact]
        public void OrderedMap_AddOrUpdate_PassesOldValueForExistingItem()
        {
            var dict = new ConcurrentOrderedDictionary<string, string>();
            dict.AddOrUpdate("foo", "bar", (k, v) => "baz");

            string callbackValue = null;
            dict.AddOrUpdate("foo", "baz", (k, v) => {
                callbackValue = v;
                return "baz";
            });

            Assert.Equal("bar", callbackValue);
        }

        [Fact]
        public void OrderedMap_ContainsKey_ReturnsTrueIfKeyExists()
        {
            const string expectedKey = "foo";

            var dict = new OrderedDictionary<string, string>();
            dict.Add(expectedKey, "bar");
            Assert.True(dict.ContainsKey(expectedKey));
        }

        [Fact]
        public void OrderedMap_ContainsKey_ReturnsFalseIfKeyDoesNotExist()
        {
            var dict = new OrderedDictionary<string, string>();
            Assert.False(dict.ContainsKey("foo"));
        }

        [Fact]
        public void OrderedMap_TryGetValue_ReturnsValueOutIfKeyExists()
        {
            const string key = "foo";
            const string expectedValue = "bar";

            var dict = new OrderedDictionary<string, string>();
            dict.Add(key, expectedValue);

            dict.TryGetValue(key, out var value);
            Assert.Equal(expectedValue, value);
        }

        [Fact]
        public void OrderedMap_TryGetValue_ReturnsTrueIfKeyExists()
        {
            const string key = "foo";

            var dict = new OrderedDictionary<string, string>();
            dict.Add(key, "bar");

            Assert.True(dict.TryGetValue(key, out var value));
        }

        [Fact]
        public void OrderedMap_TryGetValue_ReturnsFalseIfKeyDoesNotExist()
        {
            var dict = new OrderedDictionary<string, string>();
            Assert.False(dict.TryGetValue("foo", out var value));
        }
    }
}
