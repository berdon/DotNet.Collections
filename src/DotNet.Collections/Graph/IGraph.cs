using System.Collections.Generic;

namespace DotNet.Collections
{
    public interface IGraph<TVertex, TEdge>
        where TVertex : IVertex
        where TEdge : IEdge
    {
        IEnumerable<IVertex> Vertices { get; }
        IEnumerable<IEdge> Edges { get; }
    }

    public interface IVertex
    {
        IEnumerable<IEdge> Edges { get; }
        IEnumerable<IVertex> Neighbors { get; }
        IVertex AddNeighbor(IEdge edge, IVertex vertex);
        bool RemoveNeighbor(IVertex vertex);
    }

    public interface IEdge
    {
        IVertex From { get; }
        IVertex To { get; }
    }
}