using MPI;

namespace Sort;

internal class OddEvenSorter
{
    private readonly Intracommunicator _communicator;

    private int[]? _myArray;

    public OddEvenSorter(Intracommunicator communicator)
    {
        _communicator = communicator;
    }

    /// <summary>
    /// Sort array of numbers using MPI. 
    /// </summary>
    /// <param name="array">
    /// For the main process, an array of numbers to be sorted, for other processes, null<see langword="null"/>
    /// </param>
    /// <returns>
    /// For the main process, a sorted array of numbers, for other processes, <see langword="null"/> 
    /// </returns>
    public int[]? Sort(int[]? array)
    {
        // Validate input
        var shouldFinish = false;
        if (_communicator.IsMainProcess())
        {
            if (array == null)
            {
                _communicator.Broadcast(ref shouldFinish, 0);
                throw new ArgumentException("Argument array must not be null for \"root\" process");
            }

            if (array.Length < _communicator.Size)
            {
                // Input array is too small. 
                shouldFinish = true;
                // Notify other processes
                _communicator.Broadcast(ref shouldFinish, 0);
                Array.Sort(array);
                return array;
            }
        }

        _communicator.Broadcast(ref shouldFinish, 0);
        if (shouldFinish)
            return null;

        // Send array parts to their processes
        var chunks = _communicator.IsMainProcess()
            ? array?.SplitFor(_communicator.Size)
            : null;
        _myArray = _communicator.Scatter(chunks, 0);

        // Sort array
        Array.Sort(_myArray);
        OddEvenSort();

        return _communicator.Reduce(_myArray, (arr1, arr2) => arr1.Concat(arr2).ToArray(), 0);
    }

    private void OddEvenSort()
    {
        for (var i = 0; i < _communicator.Size; i++)
        {
            if (i % 2 == 0)
            {
                if (_communicator.Rank % 2 == 0 && _communicator.Rank > 0)
                    CompareAndSplit(_communicator.Rank - 1, i);
                else if (_communicator.Rank % 2 == 1 && _communicator.Rank < _communicator.Size - 1)
                    CompareAndSplit(_communicator.Rank + 1, i);
            }
            else
            {
                if (_communicator.Rank % 2 == 1 && _communicator.Rank > 0)
                    CompareAndSplit(_communicator.Rank - 1, i);
                else if (_communicator.Rank % 2 == 0 && _communicator.Rank < _communicator.Size - 1)
                    CompareAndSplit(_communicator.Rank + 1, i);
            }

            _communicator.Barrier();
        }
    }

    private void CompareAndSplit(int otherRank, int iteration)
    {
        if (_communicator.Rank < otherRank)
        {
            var myMax = _myArray![^1];
            _communicator.SendReceive(myMax, otherRank, iteration, out var otherMin);
            if (myMax < otherMin)
            {
                return;
            }
        }
        else
        {
            var myMin = _myArray![0];
            _communicator.SendReceive(myMin, otherRank, iteration, out var otherMax);
            if (myMin > otherMax)
            {
                return;
            }
        }

        _communicator.SendReceive(_myArray, otherRank, iteration, out var otherArray);

        var mergedArray = new int[_myArray.Length + otherArray.Length];
        var iMy = 0;
        var iOther = 0;
        for (var i = 0; i < mergedArray.Length; i++)
        {
            if (iMy < _myArray.Length && iOther < otherArray.Length)
            {
                mergedArray[i] = _myArray[iMy] < otherArray[iOther]
                    ? _myArray[iMy++]
                    : otherArray[iOther++];
            }
            else if (iMy < _myArray.Length)
            {
                mergedArray[i] = _myArray[iMy++];
            }
            else
            {
                mergedArray[i] = otherArray[iOther++];
            }
        }

        var offset = _communicator.Rank < otherRank
            ? 0
            : otherArray.Length;
        Array.Copy(mergedArray, offset, _myArray, 0, _myArray.Length);
    }
}
