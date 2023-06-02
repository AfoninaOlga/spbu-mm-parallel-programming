using PrimAlgorithm.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimAlgorithm.MinimumWeightEdges
{
    internal class MinimumWeightEdge
    {
        // Finding Minimum Weight Edge
        public static Edge GetMinimumWeightEdge(List<Edge> edges, bool[] verticesInTree)
        {
            var lowWeightEdge = new Edge(0, 0, int.MaxValue);

            foreach (var edge in edges)
            {
                bool isSelected = verticesInTree[edge.VertexFrom] ^ verticesInTree[edge.VertexTo];

                if (isSelected && edge.Weight < lowWeightEdge.Weight)
                {
                    lowWeightEdge = edge;
                }
            }

            return lowWeightEdge;
        }
    }
}
