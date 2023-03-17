using MPI;
using Task1;

using var environment = new MPI.Environment(ref args);
var comm = Communicator.world;

try
{
    if (comm.Rank == 0)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("There should be exactly two command line arguments: " +
                                        "input file path and output file path");
        }

        if (!File.Exists(args[0]))
        {
            throw new ArgumentException("Input file doesn't exist");
        }

        if (File.Exists(args[1]))
        {
            throw new ArgumentException("Output file already exists");
        }
    }

    comm.Barrier();
    var quickSort = new ParallelQuickSort(comm);
    quickSort.Run(args[0], args[1]);
}
catch (Exception e)
{
    Console.WriteLine($"ERROR [{comm.Rank}]: {e.Message}");
    comm.Abort(-1);
}
