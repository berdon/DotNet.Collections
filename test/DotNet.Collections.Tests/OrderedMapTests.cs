using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.Equal("bar", result);

            result = dict.AddOrUpdate("foo", "baz", (k, v) => "baz");

            Assert.Single(dict);
            Assert.Equal("baz", result);
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

        [Fact]
        public void OrderedMap_OrdersKeys_ByInsertionOrder()
        {
            const string firstKey = "foo";
            const string secondKey = "bar";

            var dict = new OrderedDictionary<string, string>();
            dict.Add(firstKey, "bar");
            dict.Add(secondKey, "foo");

            var expectedOrder = new[] { firstKey, secondKey };
            Assert.Equal(expectedOrder, dict.Keys);
        }

        [Fact]
        public void OrderedMap_OrdersValues_ByInsertionOrder()
        {
            const string firstValue = "foo";
            const string secondValue = "bar";

            var dict = new OrderedDictionary<string, string>();
            dict.Add("foo", firstValue);
            dict.Add("bar", secondValue);

            var expectedOrder = new[] { firstValue, secondValue };
            Assert.Equal(expectedOrder, dict.Values);
        }

        [Fact]
        public void OrderedMap_EnumeratesValues_ByInsertionOrder()
        {
            const string firstValue = "foo";
            const string secondValue = "bar";

            var dict = new OrderedDictionary<string, string>();
            dict.Add("foo", firstValue);
            dict.Add("bar", secondValue);

            using (IEnumerator<string> enumerator = dict.GetEnumerator())
            {
                enumerator.MoveNext();
                Assert.Equal(firstValue, enumerator.Current);
                enumerator.MoveNext();
                Assert.Equal(secondValue, enumerator.Current);
            }
        }

        [Fact]
        public void OrderedMap_CopiesTo_ByInsertionOrder()
        {
            IEnumerable<string> expectedValues = Enumerable.Range(0, 10).Reverse().Select(v => v.ToString());

            var dict = new OrderedDictionary<string, string>();
            foreach (var val in expectedValues)
            {
                dict.Add(val, val);
            }

            var copiedArray = new string[10];
            dict.CopyTo(copiedArray, 0);

            Assert.Equal(expectedValues, copiedArray);
        }

        [Fact]
        public void OrderedMap_CopiesTo_ByInsertionOrder_AndRespectsArrayIndex()
        {
            IEnumerable<string> expectedValues = Enumerable.Range(0, 10).Reverse().Select(v => v.ToString());

            var dict = new OrderedDictionary<string, string>();
            foreach (var val in expectedValues)
            {
                dict.Add(val, val);
            }

            var copiedArray = new string[15];
            dict.CopyTo(copiedArray, 5);

            Assert.Equal(new string[5], copiedArray.Take(5));
            Assert.Equal(expectedValues, copiedArray.Skip(5));
        }
    }
}
