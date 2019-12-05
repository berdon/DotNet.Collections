using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Collections
{
    public class SplayTreeTests
    {
        private readonly ITestOutputHelper _output;

        public SplayTreeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task PreFixTraversal()
        {
            var tree = new SplayTree<int> { 10, 6, 3, 11 };
            await tree.DepthFirstTraversalAsync(
                preFix: async node => _output.WriteLine(node.Value.ToString())
            );
        }

        [Fact]
        public async Task InFixTraversal()
        {
            var tree = new SplayTree<int> { 10, 6, 3, 11 };
            await tree.DepthFirstTraversalAsync(
                inFix: async node => _output.WriteLine(node.Value.ToString())
            );
        }

        [Fact]
        public async Task PostFixTraversal()
        {
            var tree = new SplayTree<int> { 10, 6, 3, 11 };
            await tree.DepthFirstTraversalAsync(
                postFix: async node => _output.WriteLine(node.Value.ToString())
            );
        }

        [Fact]
        public void CopyToTest()
        {
            var tree = new SplayTree<int>();
            var random = new Random();
            for (var i = 0; i < 100; i++)
                tree.Add(random.Next(0, 100));
            
            var array = new int[tree.Count];
            tree.CopyTo(array, 0);
        }

        [Fact]
        public void EnumerationTest()
        {
            var tree = new SplayTree<int>();
            var random = new Random();
            var expected = new HashSet<int>();
            for (var i = 0; i < 100; i++)
            {
                var randomInt = random.Next(0, 100);
                if (!expected.Add(randomInt)) i--;
                else tree.Add(randomInt);
            }            

            foreach (var i in tree)
                Assert.Contains(i, expected);
        }

        [Fact]
        public void AddTest()
        {
            new SplayTree<int> { 10, 6, 3, 11 };
        }

        [Fact]
        public void RemoveLastNodeLeavesEmptyTree()
        {
            var tree = new SplayTree<int> { 5, 10 };
            tree.Remove(10);
            Assert.Single(tree);
        }

        [Fact]
        public void RemoveSecondToLastNodeLeavesEmptyTree()
        {
            var tree = new SplayTree<int> { 5, 10, 7 };
            tree.Remove(10);
            Assert.Equal(2, tree.Count);
        }

        [Fact]
        public void LeftZigProducesExpectedResults()
        {
            var tree = new SplayTree<int>();
            var p = new BinaryTreeNode<int>(10);
            var x = new BinaryTreeNode<int>(8);
            var a = new BinaryTreeNode<int>(7);
            var b = new BinaryTreeNode<int>(9);
            var c = new BinaryTreeNode<int>(11);
            tree.TestHook_Root = p;
            p.Left = x;
            p.Right = c;
            x.Left = a;
            x.Right = b;

            tree.TestHook_Zig(x);

            Assert.Equal(x, tree.TestHook_Root);
            Assert.Null(x.Parent);
            Assert.Equal(a, x.Left);
            Assert.Equal(b, p.Left);
            Assert.Equal(c, p.Right);
        }

        [Fact]
        public void RightZigProducesExpectedResults()
        {
            var tree = new SplayTree<int>();
            var p = new BinaryTreeNode<int>(10);
            var x = new BinaryTreeNode<int>(12);
            var a = new BinaryTreeNode<int>(7);
            var b = new BinaryTreeNode<int>(11);
            var c = new BinaryTreeNode<int>(13);
            tree.TestHook_Root = p;
            p.Left = a;
            p.Right = x;
            x.Left = b;
            x.Right = c;

            tree.TestHook_Zig(x);

            Assert.Equal(x, tree.TestHook_Root);
            Assert.Null(x.Parent);
            Assert.Equal(p, x.Left);
            Assert.Equal(a, p.Left);
            Assert.Equal(b, p.Right);
            Assert.Equal(c, x.Right);
        }

        [Fact]
        public void LeftZigZigProducesExpectedResults()
        {
            var tree = new SplayTree<int>();
            var root = new BinaryTreeNode<int>(15);
            var g = new BinaryTreeNode<int>(13);
            var p = new BinaryTreeNode<int>(10);
            var x = new BinaryTreeNode<int>(8);
            var a = new BinaryTreeNode<int>(7);
            var b = new BinaryTreeNode<int>(9);
            var c = new BinaryTreeNode<int>(11);
            var d = new BinaryTreeNode<int>(14);
            tree.TestHook_Root = root;
            root.Left = g;
            g.Right = d;
            g.Left = p;
            p.Right = c;
            p.Left = x;
            x.Left = a;
            x.Right = b;

            tree.TestHook_ZigZig(x);

            Assert.Equal(root, tree.TestHook_Root);
            Assert.Equal(x, root.Left);
            Assert.Equal(a, x.Left);
            Assert.Equal(p, x.Right);
            Assert.Equal(b, p.Left);
            Assert.Equal(g, p.Right);
            Assert.Equal(c, g.Left);
            Assert.Equal(d, g.Right);
            Assert.Null(root.Parent);
        }

        [Fact]
        public void RightZigZigProducesExpectedResults()
        {
            var tree = new SplayTree<int>();
            var root = new BinaryTreeNode<int>(10);
            var g = new BinaryTreeNode<int>(13);
            var p = new BinaryTreeNode<int>(5);
            var x = new BinaryTreeNode<int>(8);
            var a = new BinaryTreeNode<int>(1);
            var b = new BinaryTreeNode<int>(3);
            var c = new BinaryTreeNode<int>(6);
            var d = new BinaryTreeNode<int>(14);
            tree.TestHook_Root = root;
            root.Right = g;
            g.Right = p;
            g.Left = a;
            p.Right = x;
            p.Left = b;
            x.Left = c;
            x.Right = d;

            tree.TestHook_ZigZig(x);

            Assert.Equal(root, tree.TestHook_Root);
            Assert.Equal(x, root.Right);
            Assert.Equal(p, x.Left);
            Assert.Equal(d, x.Right);
            Assert.Equal(g, p.Left);
            Assert.Equal(c, p.Right);
            Assert.Equal(a, g.Left);
            Assert.Equal(b, g.Right);
            Assert.Null(root.Parent);
        }

        [Fact]
        public void LeftZigZagProducesExpectedResults()
        {
            var tree = new SplayTree<int>();
            var root = new BinaryTreeNode<int>(20);
            var g = new BinaryTreeNode<int>(15);
            var p = new BinaryTreeNode<int>(10);
            var x = new BinaryTreeNode<int>(13);
            var a = new BinaryTreeNode<int>(7);
            var b = new BinaryTreeNode<int>(11);
            var c = new BinaryTreeNode<int>(14);
            var d = new BinaryTreeNode<int>(16);
            tree.TestHook_Root = root;
            root.Left = g;
            g.Right = d;
            g.Left = p;
            p.Right = x;
            p.Left = a;
            x.Left = b;
            x.Right = c;

            tree.TestHook_ZigZag(x);

            Assert.Equal(root, tree.TestHook_Root);
            Assert.Equal(x, root.Left);
            Assert.Equal(p, x.Left);
            Assert.Equal(g, x.Right);
            Assert.Equal(a, p.Left);
            Assert.Equal(b, p.Right);
            Assert.Equal(c, g.Left);
            Assert.Equal(d, g.Right);
            Assert.Null(root.Parent);
        }

        [Fact]
        public void RightZigZagProducesExpectedResults()
        {
            var tree = new SplayTree<int>();
            var root = new BinaryTreeNode<int>(-20);
            var g = new BinaryTreeNode<int>(-15);
            var p = new BinaryTreeNode<int>(-10);
            var x = new BinaryTreeNode<int>(-13);
            var a = new BinaryTreeNode<int>(-7);
            var b = new BinaryTreeNode<int>(-11);
            var c = new BinaryTreeNode<int>(-14);
            var d = new BinaryTreeNode<int>(-16);
            tree.TestHook_Root = root;
            root.Left = g;
            g.Left = d;
            g.Right = p;
            p.Left = x;
            p.Right = a;
            x.Right = b;
            x.Left = c;

            tree.TestHook_ZigZag(x);

            Assert.Equal(root, tree.TestHook_Root);
            Assert.Equal(x, root.Left);
            Assert.Equal(p, x.Right);
            Assert.Equal(g, x.Left);
            Assert.Equal(a, p.Right);
            Assert.Equal(b, p.Left);
            Assert.Equal(c, g.Right);
            Assert.Equal(d, g.Left);
            Assert.Null(root.Parent);
        }
    }
}