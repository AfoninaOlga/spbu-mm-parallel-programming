using MPI;

namespace Sort;

internal static class Utils
{
    /// <summary>
    /// Numbers delimiter in the input file
    /// </summary>
    private const string Delimiter = " ";

    /// <summary>
    /// Read array of integer numbers from the first line of the file
    /// </summary>
    /// <param name="path">Path to file contains numbers</param>
    /// <returns>Array of numbers</returns>
    /// <exception cref="FormatException">File contains no lines</exception>
    internal static int[] ReadArray(string path)
    {
        using var reader = new StreamReader(path);
        var array = reader.ReadLine()?.Split(Delimiter).Select(int.Parse).ToArray();
        if (array == null)
        {
            throw new FormatException("Input file contains no lines");
        }

        return array;
    }

    /// <summary>
    /// Write array of numbers to file separated by space
    /// </summary>
    /// <param name="array"></param>
    /// <param name="path">Path to output file</param>
    internal static void WriteArray(int[] array, string path)
    {
        using var writer = new StreamWriter(path);
        writer.WriteLine(String.Join(' ', array));
    }

    /// <summary>
    /// Splits the array into a specified number of chunks, so that
    /// their capacities differed by no more than 1.
    /// </summary>
    /// <param name="source">The array to be processed.</param>
    /// <param name="chunksCount">The number of pieces.</param>
    /// <returns>
    /// Partitioning of the given size.
    /// </returns>
    internal static T[][] SplitFor<T>(this T[] source, int chunksCount)
    {
        var chunks = new T[chunksCount][];
        var bigChunksCount = source.Length % chunksCount;
        var smallChunkLength = source.Length / chunksCount;
        var sourceOffset = 0;
        for (var i = 0; i < chunksCount; i++)
        {
            var chunkLength = i < bigChunksCount ? smallChunkLength + 1 : smallChunkLength;
            chunks[i] = new T[chunkLength];
            Array.Copy(source, sourceOffset, chunks[i], 0, chunkLength);
            sourceOffset += chunkLength;
        }

        return chunks;
    }

    /// <summary>
    /// Check current process is "root"
    /// </summary>
    /// <param name="communicator">MPI communicator</param>
    /// <returns>
    /// <see langword="true"/> if current process is "root", <see langword="false"/> otherwise 
    /// </returns>
    internal static bool IsMainProcess(this Communicator communicator) => communicator.Rank == 0;
}
