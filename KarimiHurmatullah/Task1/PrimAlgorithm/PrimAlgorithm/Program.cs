using PrimAlgorithm.Edges;
using PrimAlgorithm.MinimumWeightEdges;
using MPI;

namespace PrimAlgorithm
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (new MPI.Environment(ref args))
            {
                var comm = Communicator.world;
                var commSize = comm.Size;
                var expectedArgs = 2;

                if (args.Length != expectedArgs)
                {
                    if (comm.Rank == 0)
                    {
                        Console.Error.WriteLine($"Error! Expected {expectedArgs} arguments - input file and output file.");
                    }
                    return;
                }

                if (comm.Rank == 0)
                {
                    var inputFilePath = args[0];
                    var outputFilePath = args[1];

                    StreamReader fileReader = new StreamReader(inputFilePath);
                    var numVertices = Convert.ToInt32(fileReader.ReadLine());

                    for (int i = 1; i < commSize; i++)
                    {
                        comm.Send(numVertices, i, 0);
                    }

                    // Creates a list to store all edges for each process in the communicator

                    List<List<Edge>> allEdgesPerCommunicator = new List<List<Edge>>(commSize);

                    for (int i = 0; i < commSize; i++)
                    {
                        allEdgesPerCommunicator.Add(new List<Edge>());
                    }

                    int currentRank = 0;
                    string? line;

                    while ((line = fileReader.ReadLine()) != null)
                    {
                        string[] tokens = line.Split(' ');

                        if (tokens.Length < 3)
                        {
                            // handle invalid input format
                            continue;
                        }

                        int vertexFrom, vertexTo, weight;

                        // Checks if the input format is invalid

                        if (!int.TryParse(tokens[0], out vertexFrom) ||
                            !int.TryParse(tokens[1], out vertexTo) ||
                            !int.TryParse(tokens[2], out weight))
                        {
                            // handle invalid input format  
                            continue;
                        }

                        Edge edge = new Edge(vertexFrom, vertexTo, weight);
                        allEdgesPerCommunicator[currentRank].Add(edge);
                        currentRank = (currentRank + 1) % commSize;
                    }

                    fileReader.Close();

                    for (int i = 1; i < commSize; i++)
                    {
                        comm.Send(allEdgesPerCommunicator[i], i, 0);
                    }

                    var verticesInTree = new bool[numVertices];
                    verticesInTree[0] = true;

                    var edges = allEdgesPerCommunicator[0];
                    var result = 0;

                    for (int step = 1; step < numVertices; step++)
                    {
                        // Find minimum weight of edges

                        var lowWeightEdge = MinimumWeightEdge.GetMinimumWeightEdge(edges, verticesInTree);

                        for (int i = 1; i < commSize; i++)
                        {
                            var receivedMinEdge = comm.Receive<Edge>(i, 0);
                            if (receivedMinEdge.Weight < lowWeightEdge.Weight)
                            {
                                lowWeightEdge = receivedMinEdge;
                            }
                        }

                        result += lowWeightEdge.Weight;
                        verticesInTree[lowWeightEdge.VertexFrom] = true;
                        verticesInTree[lowWeightEdge.VertexTo] = true;

                        for (int i = 1; i < commSize; i++)
                        {
                            comm.Send(lowWeightEdge, i, 0);
                        }
                    }

                    StreamWriter writer = new StreamWriter(outputFilePath);
                    writer.WriteLine(numVertices);
                    writer.Write(result);
                    writer.Close();
                }
                else
                {
                    var numVertices = comm.Receive<int>(0, 0);
                    var edges = comm.Receive<List<Edge>>(0, 0);

                    var verticesInTree = new bool[numVertices];
                    verticesInTree[0] = true;

                    for (int step = 1; step < numVertices; step++)
                    {
                        // Find minimum weight of edges

                        var lowWeightEdge = MinimumWeightEdge.GetMinimumWeightEdge(edges, verticesInTree);

                        comm.Send(lowWeightEdge, 0, 0);

                        var newEdgeInTree = comm.Receive<Edge>(0, 0);
                        verticesInTree[newEdgeInTree.VertexFrom] = true;
                        verticesInTree[newEdgeInTree.VertexTo] = true;
                    }
                }
            }
        }
    }
}