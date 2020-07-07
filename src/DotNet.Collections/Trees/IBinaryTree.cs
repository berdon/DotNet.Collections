using System;
using System.Threading.Tasks;

namespace DotNet.Collections
{
    public interface IBinaryTree<T> : ITree<T>
        where T : IComparable<T>
    {
        BinaryTreeNode<T> Find(T value);
        void PreFixTraversal(Action<BinaryTreeNode<T>> callback);
        void InFixTraversal(Action<BinaryTreeNode<T>> callback);
        void PostFixTraversal(Action<BinaryTreeNode<T>> callback);
        Task PreFixTraversalAsync(Func<BinaryTreeNode<T>, Task> callback);
        Task InFixTraversalAsync(Func<BinaryTreeNode<T>, Task> callback);
        Task PostFixTraversalAsync(Func<BinaryTreeNode<T>, Task> callback);
    }
}