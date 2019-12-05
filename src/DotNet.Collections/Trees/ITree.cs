using System;
using System.Collections.Generic;

namespace DotNet.Collections
{
    public interface ITree<T> : ICollection<T>
        where T : IComparable<T>
    {
    }
}