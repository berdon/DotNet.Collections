using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Collections;
using Xunit;
using Xunit.Abstractions;

namespace DotNet.Collections
{
    public class WeightedGraphTests
    {
        private readonly ITestOutputHelper _output;

        public WeightedGraphTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CanCreateGraph()
        {
            var graph = new WeightedGraph<string, FloatWeight>("Sioux Falls");
            var start = graph.Root;
            var a = start.AddCyclicNeighbor(10, "A");
            var b = a.AddCyclicNeighbor(10, "B");
            var c = b.AddCyclicNeighbor(10, "C");
            c.AddCyclicNeighbor(10, "Rapid City");
            var madison = start.AddCyclicNeighbor(51.0f, "Madison");
            var rapidCity = start.AddCyclicNeighbor(347.2f, "Rapid City");
            var mobridge = madison.AddCyclicNeighbor(260.0f, "Mobridge");
            mobridge.AddCyclicNeighbor(235.6f, rapidCity.Value);

            var result = graph.ShortestPath(start, rapidCity);
        }

        public struct FloatWeight : IWeight
        {
            private readonly float _weight;

            public FloatWeight(float weight)
            {
                _weight = weight;
            }

            public IWeight Add(IWeight weight)
            {
                return new FloatWeight(((FloatWeight) weight)._weight + _weight);
            }

            public int CompareTo(IWeight other)
            {
                return _weight.CompareTo(((FloatWeight) other)._weight);
            }

            public static implicit operator float(FloatWeight weight) => weight._weight;
            public static implicit operator FloatWeight(float weight) => new FloatWeight(weight);
        }
    }

    public static class GraphExtensions
    {
        public static WeightedGraph<TValue, TWeight>.Vertex AddCyclicNeighbor<TWeight, TValue>(this WeightedGraph<TValue, TWeight>.Vertex self, TWeight weight, TValue value)
            where TWeight : struct, IWeight
        {
            var neighbor = self.AddNeighbor(weight, value);
            neighbor.AddNeighbor(weight, self.Value);
            return neighbor;
        }
    }
}