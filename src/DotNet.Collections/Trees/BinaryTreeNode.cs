using System;
using System.Collections.Generic;

namespace DotNet.Collections
{
    public class BinaryTreeNode<T> where T : IComparable<T>
    {
        private BinaryTreeNode<T> _left;
        private BinaryTreeNode<T> _right;

        public BinaryTreeNode<T> Parent { get; set; }
        public T Value { get; set; }
        public BinaryTreeNode<T> Left
        {
            get => _left;
            set
            {
                _left = value;
                if (_left != null)
                    _left.Parent = this;
            }
        }
        public BinaryTreeNode<T> Right
        {
            get => _right;
            set
            {
                _right = value;
                if (_right != null)
                    _right.Parent = this;
            }
        }

        public static bool operator >(BinaryTreeNode<T> a, BinaryTreeNode<T> b) => a.Value.CompareTo(b.Value) > 0;
        public static bool operator >=(BinaryTreeNode<T> a, BinaryTreeNode<T> b) => a.Value.CompareTo(b.Value) >= 0;
        public static bool operator <(BinaryTreeNode<T> a, BinaryTreeNode<T> b) => a.Value.CompareTo(b.Value) < 0;
        public static bool operator <=(BinaryTreeNode<T> a, BinaryTreeNode<T> b) => a.Value.CompareTo(b.Value) <= 0;

        internal bool IsLeftChild => Parent?.Left == this;
        internal bool IsRightChild => Parent?.Right == this;
        internal bool HasChildren => Left != null && Right != null;
        internal BinaryTreeNode<T> GrandParent => Parent?.Parent;

        public BinaryTreeNode(T value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is BinaryTreeNode<T> node &&
                    EqualityComparer<BinaryTreeNode<T>>.Default.Equals(Parent, node.Parent) &&
                    EqualityComparer<T>.Default.Equals(Value, node.Value) &&
                    EqualityComparer<BinaryTreeNode<T>>.Default.Equals(GrandParent, node.GrandParent);
        }

        public override int GetHashCode()
        {
            var hashCode = -933820509;
            hashCode = hashCode * -1521134295 + EqualityComparer<BinaryTreeNode<T>>.Default.GetHashCode(Parent);
            hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Value);
            hashCode = hashCode * -1521134295 + EqualityComparer<BinaryTreeNode<T>>.Default.GetHashCode(GrandParent);
            return hashCode;
        }
    }
}