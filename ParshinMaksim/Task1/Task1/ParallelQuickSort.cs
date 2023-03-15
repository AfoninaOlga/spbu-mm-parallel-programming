using System.Diagnostics.CodeAnalysis;
using System.Text;
using MPI;

namespace Task1;

public class ParallelQuickSort
{
    private const int SlaveProcessPivotsTag = 111;
    private const int RefinedPivotsTag = 222;
    private const int ChunkTag = 333;
    private const int ResultTag = 444;

    private readonly Intracommunicator _comm;
    private readonly int _procCount;
    private readonly int _myProcNumber;
    private readonly bool _iAmMaster;

    public ParallelQuickSort(Intracommunicator communicator)
    {
        _comm = communicator;
        _procCount = communicator.Size;
        _myProcNumber = communicator.Rank;
        _iAmMaster = _myProcNumber == 0;
    }

    public void Run(string inputFilePath, string outputFilePath)
    {
        var numbers = ReadInputArray(inputFilePath).ToArray();
        if (numbers.Length == 0)
        {
            throw new InvalidDataException("-n greater than the array length is not supported");
        }

        Array.Sort(numbers);

        var totalArrayLength = _comm.Allreduce(numbers.Length, Operation<int>.Add);
        var pivotStep = totalArrayLength / (_procCount * _procCount);

        var pivotsList = new List<int>();
        for (var i = 0; i < _procCount; i++)
        {
            pivotsList.Add(numbers[i * pivotStep]);
        }

        var pivots = pivotsList.ToArray();
        var refinedPivots = RefinePivots(pivots);

        var chunks = new int[_procCount][];
        var (myChunk, sendRequests) = SendChunks(numbers, refinedPivots);
        chunks[_myProcNumber] = myChunk;

        // We don't need input array anymore
        Array.Clear(numbers);

        var chunksTotalLength = ReceiveChunks(chunks);
        sendRequests.WaitAll();

        var sorted = Merge(chunksTotalLength, chunks);
        WriteToFile(sorted, outputFilePath);
    }

    /// <summary>
    /// Reads input array from file. Different processes read different parts of array
    /// simultaneously
    /// </summary>
    /// <param name="path">Input file path</param>
    /// <returns>Read array elements</returns>
    /// <exception cref="InvalidDataException">Data in file is incorrect</exception>
    private List<int> ReadInputArray(string path)
    {
        using var reader = new StreamReader(path, Encoding.ASCII);
        var numbers = new List<int>();
        var readNums = 0;

        for (var i = 0; !reader.EndOfStream; ++i)
        {
            if (i % _procCount != _myProcNumber)
            {
                while (!reader.EndOfStream && (char)reader.Read() != ' ')
                {
                }

                continue;
            }

            var sb = new StringBuilder();

            while (!reader.EndOfStream)
            {
                var read = (char)reader.Read();
                if (read == ' ')
                {
                    break;
                }

                if (!char.IsDigit(read) && read != '-')
                {
                    throw new InvalidDataException("Unexpected character in input file");
                }

                sb.Append(read);
            }

            if (int.TryParse(sb.ToString(), out var number))
            {
                numbers.Add(number);
                readNums++;
            }
            else
            {
                throw new InvalidDataException($"Invalid {readNums} integer {sb}");
            }

        }

        return numbers;
    }

    /// <summary>
    /// Merges multiple sorted arrays into one (like in merge sort)
    /// </summary>
    /// <param name="totalCount">Total number of elements in all arrays</param>
    /// <param name="elements">Array of sorted arrays</param>
    /// <returns>Merged array</returns>
    private int[] Merge(int totalCount, int[][] elements)
    {
        var result = new int[totalCount];
        var currentElement = new int[elements.Length];

        for (var i = 0; i < totalCount; ++i)
        {
            var minIndex = -1;
            var minValue = int.MaxValue;
            for (var j = 0; j < elements.Length; ++j)
            {
                if (currentElement[j] < elements[j].Length && elements[j][currentElement[j]] < minValue)
                {
                    minIndex = j;
                    minValue = elements[j][currentElement[j]];
                }
            }

            if (minIndex == -1)
            {
                break;
            }

            result[i] = minValue;
            currentElement[minIndex]++;
        }

        return result;
    }

    /// <summary>
    /// Gathers preliminary pivot elements from each processor and produces pivot elements common for all subarrays
    /// </summary>
    /// <param name="pivots">Pivot elements from current process data</param>
    /// <returns>Refined pivots</returns>
    private int[] RefinePivots(int[] pivots)
    {
        int[] refinedPivots;

        if (!_iAmMaster)
        {
            _comm.Send(pivots, 0, SlaveProcessPivotsTag);
            _comm.ReceiveUnknownSize(0, RefinedPivotsTag, out refinedPivots);
            return refinedPivots;
        }

        var pivotsToMerge = new int[_procCount][];
        pivotsToMerge[0] = pivots;
        var pivotsCount = pivots.Length;

        for (int i = 1; i < _procCount; i++)
        {
            var size = _comm.ReceiveUnknownSize(i, SlaveProcessPivotsTag, out pivotsToMerge[i]);
            pivotsCount += size;
        }
        var mergedPivots = Merge(pivotsCount, pivotsToMerge);

        var refinedPivotsList = new List<int>();
        for (var i = _procCount + _procCount / 2 - 1; i < mergedPivots.Length; i += _procCount)
        {
            refinedPivotsList.Add(mergedPivots[i]);
        }
        refinedPivots = refinedPivotsList.ToArray();

        for (var i = 1; i < _procCount; ++i)
        {
            _comm.Send(refinedPivots, i, RefinedPivotsTag);
        }

        return refinedPivots;
    }

    /// <summary>
    /// Splits array on each processor into chunks according to pivot elements and sends them to other processors
    /// </summary>
    /// <param name="numbers">Array to split into chunks</param>
    /// <param name="refinedPivots">Refined pivot elements produced by RefinePivots</param>
    /// <returns>Chunk, corresponding to the current process (which should not be sent) and send requests for the remaining chunks</returns>
    private (int[], RequestList) SendChunks(int[] numbers, int[] refinedPivots)
    {
        var myChunk = Array.Empty<int>();
        var requests = new RequestList();

        var left = 0;

        for (var i = 0; i < _procCount; i++)
        {
            var right = i != _procCount - 1 ? Array.BinarySearch(numbers, refinedPivots[i]) : numbers.Length;
            if (right < 0)
            {
                right = ~right;
            }
            else
            {
                while (right < numbers.Length && numbers[right] == refinedPivots[i])
                {
                    right++;
                }
            }

            var chunk = numbers[left..right];

            if (i == _myProcNumber)
            {
                myChunk = chunk;
            }
            else
            {
                var request = _comm.ImmediateSend(chunk, i, ChunkTag);
                requests.Add(request);
            }

            left = right;
        }

        return (myChunk, requests);
    }

    /// <summary>
    /// Receives chunks from all processor and saves them to buffer
    /// </summary>
    /// <param name="chunks">Received chunks buffer</param>
    /// <returns>Total number of elements in all chunks</returns>
    private int ReceiveChunks(int[][] chunks)
    {
        var chunksTotalLength = chunks[_myProcNumber].Length;

        for (var i = 0; i < _procCount; ++i)
        {
            if (i == _myProcNumber)
            {
                continue;
            }

            var size = _comm.ReceiveUnknownSize(i, ChunkTag, out chunks[i]);
            chunksTotalLength += size;
        }

        return chunksTotalLength;
    }

    /// <summary>
    /// Sequentially gathers sorted arrays at master process and writes them to the output file
    /// </summary>
    /// <param name="mySorted">Current process' sorting result</param>
    /// <param name="path">Output file path</param>
    private void WriteToFile(int[] mySorted, string path)
    {
        if (_iAmMaster)
        {
            using var writer = new StreamWriter(path, true, Encoding.ASCII);

            void WriteArrayToFile(int[] array)
            {
                for (var i = 0; i < array.Length; ++i)
                {
                    writer?.Write(array[i]);
                    if (i != array.Length - 1)
                    {
                        writer?.Write(' ');
                    }
                }
            }

            WriteArrayToFile(mySorted);
            Array.Clear(mySorted);

            for (var i = 1; i < _procCount; ++i)
            {
                var size = _comm.ReceiveUnknownSize<int>(i, ResultTag, out var received);
                if (size > 0)
                {
                    writer.Write(' ');
                }
                WriteArrayToFile(received);
            }
        }
        else
        {
            _comm.Send(mySorted, 0, ResultTag);
        }
    }
}
