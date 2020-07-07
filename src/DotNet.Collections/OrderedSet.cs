using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Collections
{
    /// <summary>
    /// See: https://gmamaladze.wordpress.com/2013/07/25/hashset-that-preserves-insertion-order-or-net-implementation-of-linkedhashset/
    /// and https://stackoverflow.com/a/17861748/110762
    /// </summary>
    public class OrderedSet<T> : ICollection<T>, ISet<T>
    {
        private readonly IDictionary<T, LinkedListNode<T>> _dictionary;
        private readonly LinkedList<T> _linkedList;

        public OrderedSet()
            : this(Enumerable.Empty<T>(), EqualityComparer<T>.Default)
        {
        }

        public OrderedSet(IEnumerable<T> items)
            : this(items, EqualityComparer<T>.Default)
        {   
        }

        public OrderedSet(IEqualityComparer<T> comparer)
            : this(Enumerable.Empty<T>(), comparer)
        {
        }

        public OrderedSet(IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            items ??= Enumerable.Empty<T>();
            comparer ??= EqualityComparer<T>.Default;

            _dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
            _linkedList = new LinkedList<T>();

            foreach (var item in items)
                Add(item);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return _dictionary.IsReadOnly; }
        }

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        public void Clear()
        {
            _linkedList.Clear();
            _dictionary.Clear();
        }

        public bool Remove(T item)
        {
            LinkedListNode<T> node;
            bool found = _dictionary.TryGetValue(item, out node);
            if (!found) return false;
            _dictionary.Remove(item);
            _linkedList.Remove(node);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _linkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _linkedList.CopyTo(array, arrayIndex);
        }

        public bool Add(T item)
        {
            if (_dictionary.ContainsKey(item)) return false;
            LinkedListNode<T> node = _linkedList.AddLast(item);
            _dictionary.Add(item, node);
            return true;
        }

        public void ExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

        public void IntersectWith(IEnumerable<T> other) => throw new NotImplementedException();

        public bool IsProperSubsetOf(IEnumerable<T> other) => new HashSet<T>(_dictionary.Keys).IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<T> other) => new HashSet<T>(_dictionary.Keys).IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<T> other) => new HashSet<T>(_dictionary.Keys).IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<T> other) => new HashSet<T>(_dictionary.Keys).IsSupersetOf(other);

        public bool Overlaps(IEnumerable<T> other) => new HashSet<T>(_dictionary.Keys).Overlaps(other);

        public bool SetEquals(IEnumerable<T> other) => new HashSet<T>(_dictionary.Keys).SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotImplementedException();

        public void UnionWith(IEnumerable<T> other) => throw new NotImplementedException();
    }
}
