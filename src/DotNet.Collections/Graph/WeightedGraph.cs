using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DotNet.Collections
{
    public class WeightedGraph<TValue, TWeight> : IGraph<WeightedGraph<TValue, TWeight>.Vertex, WeightedEdge<TWeight>>
        where TWeight : struct, IWeight
    {
        private readonly Dictionary<Vertex, Dictionary<Vertex, WeightedEdge<TWeight>>> _vertices = new Dictionary<Vertex, Dictionary<Vertex, WeightedEdge<TWeight>>>();
        public IEnumerable<IVertex> Vertices => _vertices.Keys;
        public IEnumerable<IEdge> Edges => _vertices.Values.SelectMany(x => x.Values);
        public readonly Vertex Root;

        public WeightedGraph(TValue root)
        {
            Root = new Vertex(this, root);
            _vertices.Add(Root, new Dictionary<Vertex, WeightedEdge<TWeight>>());
        }

        public (IList<Vertex> Path, TWeight Cost) ShortestPath(Vertex start, Vertex end)
        {
            var untraveled = new HashSet<Vertex>();
            var distances = new Dictionary<Vertex, WeightWrapper>();
            var previous = new Dictionary<Vertex, Vertex>();

            foreach (var v in Vertices.Cast<Vertex>())
            {
                distances[v] = WeightWrapper.Infinite;
                previous[v] = null;
                untraveled.Add(v);
            }
            distances[start] = WeightWrapper.For(default);

            while (untraveled.Any())
            {
                var u = untraveled.OrderBy(x => distances[x]).First();
                untraveled.Remove(u);
                if (u == end) break;
                
                foreach (var neighbor in u.Neighbors)
                {
                    var contains = _vertices.ContainsKey(u);
                    var edges = _vertices[u];
                    var length = edges[neighbor as Vertex].Weight;
                    var alt = distances[u].Add(length);
                    if (alt.CompareTo(distances[neighbor as Vertex]) < 0)
                    {
                        distances[neighbor as Vertex] = alt;
                        previous[neighbor as Vertex] = u;
                    }
                }
            }

            var path = new List<Vertex>();
            var walker = end;
            while (walker != null)
            {
                path.Insert(0, walker);
                walker = previous[walker];
            }

            return (path, distances[end].Weight.Value);
        }

        public class Vertex : IVertex
        {
            private readonly WeightedGraph<TValue, TWeight> _graph;
            public readonly TValue Value;

            public IEnumerable<IEdge> Edges => _graph._vertices[this].Values;

            public IEnumerable<IVertex> Neighbors => Edges.Select(e => e.To);

            public Vertex(WeightedGraph<TValue, TWeight> graph, TValue value)
            {
                _graph = graph;
                Value = value;
            }

            public Vertex AddNeighbor(TWeight weight, TValue to)
            {
                var vertex = new Vertex(_graph, to);
                var edge = new WeightedEdge<TWeight>(this, vertex, weight);
                return AddNeighbor(edge, vertex) as Vertex;
            }

            public IVertex AddNeighbor(IEdge edge, IVertex vertex)
            {
                if (!_graph._vertices.TryGetValue(this, out var neighborNodes))
                {
                    neighborNodes = new Dictionary<Vertex, WeightedEdge<TWeight>>();
                    _graph._vertices.Add(this, neighborNodes);
                }

                if (neighborNodes.ContainsKey(vertex as Vertex)) return vertex;
                neighborNodes.Add(vertex as Vertex, edge as WeightedEdge<TWeight>);

                return vertex;
            }

            public bool RemoveNeighor(Vertex neighbor)
            {
                if (_graph._vertices.TryGetValue(this, out var neighborNodes)) return false;

                neighborNodes.Remove(neighbor);
                _graph._vertices.Remove(neighbor);

                return true;
            }

            public bool RemoveNeighbor(IVertex vertex) => RemoveNeighbor(vertex as Vertex);

            public override bool Equals(object obj)
            {
                return obj is Vertex vertex &&
                       EqualityComparer<TValue>.Default.Equals(Value, vertex.Value);
            }

            public override int GetHashCode()
            {
                int hashCode = -766605026;
                hashCode = hashCode * -1521134295 + EqualityComparer<TValue>.Default.GetHashCode(Value);
                return hashCode;
            }

            IVertex IVertex.AddNeighbor(IEdge edge, IVertex vertex)
            {
                throw new NotImplementedException();
            }

            bool IVertex.RemoveNeighbor(IVertex vertex)
            {
                throw new NotImplementedException();
            }
        }

        private struct WeightWrapper : IComparable<WeightWrapper>
        {
            public bool IsInfinite => Weight == null;
            public TWeight? Weight { get; set; }

            public readonly static WeightWrapper Infinite = new WeightWrapper();
            public static WeightWrapper For(TWeight weight) => new WeightWrapper { Weight = weight };
            public WeightWrapper Add(TWeight other) => IsInfinite ? this : WeightWrapper.For((TWeight) Weight.Value.Add(other));
            public int CompareTo(WeightWrapper other) => IsInfinite && other.IsInfinite ? 0 : (IsInfinite ? 1 : (other.IsInfinite ? -1 : Weight.Value.CompareTo(other.Weight)));
        }
    }

    public interface IWeight : IComparable<IWeight>
    {
        IWeight Add(IWeight weight);
    }

    public class WeightedEdge<TWeight> : IEdge
        where TWeight : IWeight
    {
        public IVertex From { get; private set; }
        public IVertex To { get; private set; }
        public TWeight Weight { get; private set; }

        public WeightedEdge(IVertex from, IVertex to, TWeight weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
    }
}