using System;
using System.Collections.Generic;

namespace DotNet.Collections
{
    public static class CollectionsExtensions
    {
        public static ISet<T> ToOrderedSet<T>(IEnumerable<T> items = null, Func<T, T, bool> comparer = null)
        {
            return new OrderedSet<T>(items, comparer != null ? (IEqualityComparer<T>) new Comparers.EqualityComparer<T>(comparer) : EqualityComparer<T>.Default);
        }

        public static ISet<T> ToOrderedSet<T>(IEnumerable<T> items = null, IEqualityComparer<T> comparer = null)
        {
            return new OrderedSet<T>(items, comparer);
        }
    }
}