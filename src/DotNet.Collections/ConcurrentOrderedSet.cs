using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNet.Collections
{
    public class ConcurrentOrderedSet<T> : ICollection<T>
    {
        private readonly IDictionary<T, LinkedListNode<T>> _dictionary;
        private readonly LinkedList<T> _linkedList;
        private readonly object _lock = new object();

        public ConcurrentOrderedSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public ConcurrentOrderedSet(IEqualityComparer<T> comparer)
        {
            _dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
            _linkedList = new LinkedList<T>();
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
            lock (_lock)
            {
                _linkedList.Clear();
                _dictionary.Clear();
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                LinkedListNode<T> node;
                bool found = _dictionary.TryGetValue(item, out node);
                if (!found) return false;
                _dictionary.Remove(item);
                _linkedList.Remove(node);
                return true;
            }
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
            lock (_lock)
            {
                if (_dictionary.ContainsKey(item)) return false;
                LinkedListNode<T> node = _linkedList.AddLast(item);
                _dictionary.Add(item, node);
                return true;
            }
        }
    }
}
