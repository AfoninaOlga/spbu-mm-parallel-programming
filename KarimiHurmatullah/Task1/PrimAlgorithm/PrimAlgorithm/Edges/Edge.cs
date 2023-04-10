using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimAlgorithm.Edges
{
    [Serializable]
    public struct Edge
    {
        public int VertexFrom { get; }
        public int VertexTo { get; }
        public int Weight { get; }

        public Edge(int vertexFrom, int vertexTo, int weight)
        {
            VertexFrom = vertexFrom;
            VertexTo = vertexTo;
            Weight = weight;
        }
    }
}
