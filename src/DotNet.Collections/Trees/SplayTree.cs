using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet.Collections
{
    public class SplayTree<T> : IBinaryTree<T>
        where T : IComparable<T>
    {
        private BinaryTreeNode<T> _root;
        private BinaryTreeNode<T> Root
        {
            get => _root;
            set
            {
                _root = value;
                _root.Parent = null;
            }
        }
        private int _count;
        private readonly bool _isReadOnly;
        private readonly Random _random = new Random();
        public int Count => _count;
        public bool IsReadOnly => _isReadOnly;

        public SplayTree() { }

        public void Add(T item)
        {
            Add(new BinaryTreeNode<T>(item));
        }

        public void Add(BinaryTreeNode<T> item)
        {
            if (Root == null)
            {
                Root = item;
                _count = 1;
                return;
            }

            var iterRoot = Root;
            while(true)
            {
                if (item < iterRoot)
                {
                    if (iterRoot.Left == null)
                    {
                        iterRoot.Left = item;
                        break;
                    }
                    else
                    {
                        iterRoot = iterRoot.Left;
                    }
                }
                else
                {
                    if (iterRoot.Right == null)
                    {
                        iterRoot.Right = item;
                        break;
                    }
                    else
                    {
                        iterRoot = iterRoot.Right;
                    }
                }
            }

            Splay(item);

            _count++;
        }

        private void Splay(BinaryTreeNode<T> item)
        {
            if (item == null) return;
            while (_root != item)
            {
                var parent = item.Parent;

                if (IsRoot(parent)) Zig(item);
                else if (item.IsLeftChild == parent.IsLeftChild) ZigZig(item);
                else if (item.IsRightChild == parent.IsRightChild) ZigZig(item);
                else ZigZag(item);
            }
        }

        private void Zig(BinaryTreeNode<T> item)
        {
            var parent = item.Parent;

            if (item.IsLeftChild)
            {
                var b = item.Right;

                Root = item;
                Root.Right = parent;
                parent.Left = b;
            }
            else
            {
                var b = item.Left;

                Root = item;
                Root.Left = parent;
                parent.Right = b;
            }
        }

        private void ZigZig(BinaryTreeNode<T> item)
        {
            var parent = item.Parent;
            var grandParent = item.GrandParent;

            if (item.IsLeftChild)
            {
                var b = item.Right;
                var c = parent.Right;

                if (grandParent.Parent != null)
                    if (grandParent.IsLeftChild) grandParent.Parent.Left = item;
                    else grandParent.Parent.Right = item;
                else Root = item;

                item.Right = parent;
                parent.Left = b;
                parent.Right = grandParent;
                grandParent.Left = c;
            }
            else
            {
                var b = parent.Left;
                var c = item.Left;

                if (grandParent.Parent != null)
                    if (grandParent.IsLeftChild) grandParent.Parent.Left = item;
                    else grandParent.Parent.Right = item;
                else Root = item;
                
                item.Left = parent;
                parent.Right = c;
                parent.Left = grandParent;
                grandParent.Right = b;
            }
        }

        private void ZigZag(BinaryTreeNode<T> item)
        {
            var parent = item.Parent;
            var grandParent = item.GrandParent;

            if (item.IsRightChild)
            {
                var b = item.Left;
                var c = item.Right;

                if (grandParent.Parent != null)
                    if (grandParent.IsLeftChild) grandParent.Parent.Left = item;
                    else grandParent.Parent.Right = item;
                else Root = item;

                item.Left = parent;
                item.Right = grandParent;
                parent.Right = b;
                grandParent.Left = c;
            }
            else
            {
                var b = item.Right;
                var c = item.Left;

                if (grandParent.Parent != null)
                    if (grandParent.IsLeftChild) grandParent.Parent.Left = item;
                    else grandParent.Parent.Right = item;
                else Root = item;

                item.Right = parent;
                item.Left = grandParent;
                parent.Left = b;
                grandParent.Right = c;
            }
        }

        public void Clear()
        {
            Root = null;
            _count = 0;
        }

        public bool Contains(T item)
        {
            return Find(item) != null;
        }

        public BinaryTreeNode<T> Find(T item)
        {
            if (item == null) return null;

            var iter = _root;
            while(iter != null && iter.Value.CompareTo(item) != 0)
            {
                if (iter.Value.CompareTo(item) > 0) iter = iter.Left;
                else iter = iter.Right;
            }

            return iter;
        }

        //
        // Summary:
        //     Copies the elements of the System.Collections.Generic.ICollection`1 to an System.Array,
        //     starting at a particular System.Array index.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements copied
        //     from System.Collections.Generic.ICollection`1. The System.Array must have zero-based
        //     indexing.
        //
        //   arrayIndex:
        //     The zero-based index in array at which copying begins.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     array is null.
        //
        //   T:System.ArgumentOutOfRangeException:
        //     arrayIndex is less than 0.
        //
        //   T:System.ArgumentException:
        //     The number of elements in the source System.Collections.Generic.ICollection`1
        //     is greater than the available space from arrayIndex to the end of the destination
        //     array.
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException();
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is of an insuficient size for copying the source enumerable");
            foreach (var item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        public IEnumerator<T> GetEnumerator() => new TreeDfsEnumerator(_root);

        public bool Remove(T item)
        {
            var node = Find(item);
            if (node == null) return false;
            var parent = node.Parent;
            Remove(node);
            Splay(parent);
            return true;
        }

        public void Remove(BinaryTreeNode<T> node)
        {
            if (node.Left == null && node.Right == null)
            {
                if (node.Parent == null) Root = null;
                else if (node.Parent.Left == node) node.Parent.Left = null;
                else node.Parent.Right = null;
            }
            else if (node.Left != node.Right)
            {
                var singleChildNode = node.Left ?? node.Right;
                if (node.Parent == null) Root = singleChildNode;
                else if (node.Parent.Left == node) node.Parent.Left = singleChildNode;
                else node.Parent.Right = singleChildNode;
            }
            else
            {
                var nextNode = FindNextChild(node);
                Remove(nextNode);
                if (node.Parent == null) Root = nextNode;
                nextNode.Left = node.Left;
                nextNode.Right = node.Right;
            }

            _count--;
        }

        private BinaryTreeNode<T> FindNextChild(BinaryTreeNode<T> node)
        {
            if (node == null || !node.HasChildren) return null;

            if (_random.Next(2) % 0 == 0)
            {
                if (node.Right == null) return FindMaxChild(node.Left);
                else return FindMinChild(node.Right);
            }
            else
            {
                if (node.Left == null) return FindMinChild(node.Right);
                else return FindMaxChild(node.Left);
            }
        }

        public BinaryTreeNode<T> FindMaxChild() => FindMaxChild(Root);

        private BinaryTreeNode<T> FindMaxChild(BinaryTreeNode<T> node)
        {
            if (node == null || !node.HasChildren) return null;
            while (node.Right != null) node = node.Right;
            return node;
        }

        public BinaryTreeNode<T> FindMinChild() => FindMinChild(Root);

        private BinaryTreeNode<T> FindMinChild(BinaryTreeNode<T> node)
        {
            if (node == null || !node.HasChildren) return null;
            while (node.Left != null) node = node.Left;
            return node;
        }

        IEnumerator IEnumerable.GetEnumerator() => new TreeDfsEnumerator(_root);

        private bool IsRoot(BinaryTreeNode<T> node) => Root == node;

        private class TreeDfsEnumerator : IEnumerator<T>
        {
            private readonly BinaryTreeNode<T> _root;
            private readonly Stack<BinaryTreeNode<T>> _stack = new Stack<BinaryTreeNode<T>>();
            private BinaryTreeNode<T> _current;

            public TreeDfsEnumerator(BinaryTreeNode<T> root)
            {
                _root = root;
                Reset();
            }

            public T Current => _current.Value;

            object IEnumerator.Current => _current.Value;

            public bool MoveNext()
            {
                if (!_stack.Any()) return false;
                _current = _stack.Pop();
                if (_current.Right != null) _stack.Push(_current.Right);
                if (_current.Left != null) _stack.Push(_current.Left);
                return true;
            }

            public void Reset()
            {
                _current = null;
                _stack.Clear();
                _stack.Push(_root);
            }
            public void Dispose() { }
        }

        internal void TestHook_Zig(BinaryTreeNode<T> item) => Zig(item);
        internal void TestHook_ZigZig(BinaryTreeNode<T> item) => ZigZig(item);
        internal void TestHook_ZigZag(BinaryTreeNode<T> item) => ZigZag(item);

        private void DepthFirstTraversal(Action<BinaryTreeNode<T>> preFix = null, Action<BinaryTreeNode<T>> inFix = null, Action<BinaryTreeNode<T>> postFix = null)
        {
            DepthFirstTraversalAsync(
                (node) => { preFix?.Invoke(node); return Task.CompletedTask; },
                (node) => { inFix?.Invoke(node); return Task.CompletedTask; },
                (node) => { postFix?.Invoke(node); return Task.CompletedTask; }).Wait();
        }

        private Task DepthFirstTraversalAsync(Func<BinaryTreeNode<T>, Task> preFix = null, Func<BinaryTreeNode<T>, Task> inFix = null, Func<BinaryTreeNode<T>, Task> postFix = null)
        {
            return DepthFirstTraversalAsync(Root, preFix, inFix, postFix);
        }

        private async Task DepthFirstTraversalAsync(BinaryTreeNode<T> root, Func<BinaryTreeNode<T>, Task> preFix = null, Func<BinaryTreeNode<T>, Task> inFix = null, Func<BinaryTreeNode<T>, Task> postFix = null)
        {
            await (preFix?.Invoke(root) ?? Task.CompletedTask);
            if (root.Left != null) await DepthFirstTraversalAsync(root.Left, preFix, inFix, postFix);
            await (inFix?.Invoke(root) ?? Task.CompletedTask);
            if (root.Right != null) await DepthFirstTraversalAsync(root.Right, preFix, inFix, postFix);
            await (postFix?.Invoke(root) ?? Task.CompletedTask);
        }

        public void PreFixTraversal(Action<BinaryTreeNode<T>> callback) => DepthFirstTraversal(preFix: callback);

        public void InFixTraversal(Action<BinaryTreeNode<T>> callback) => DepthFirstTraversal(inFix: callback);

        public void PostFixTraversal(Action<BinaryTreeNode<T>> callback) => DepthFirstTraversal(postFix: callback);

        public Task PreFixTraversalAsync(Func<BinaryTreeNode<T>, Task> callback) => DepthFirstTraversalAsync(preFix: callback);

        public Task InFixTraversalAsync(Func<BinaryTreeNode<T>, Task> callback) => DepthFirstTraversalAsync(inFix: callback);

        public Task PostFixTraversalAsync(Func<BinaryTreeNode<T>, Task> callback) => DepthFirstTraversalAsync(postFix: callback);

        internal BinaryTreeNode<T> TestHook_Root
        {
            get => _root;
            set => _root = value;
        }
    }
}