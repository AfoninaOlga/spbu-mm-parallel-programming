using System.Security.Cryptography;
using MPI;

namespace Task1;

public static class MPIExtensions
{
    public static int ReceiveUnknownSize<T>(this Intracommunicator comm, int source, int tag, out T[] received)
    {
        var size = comm.Probe(source, tag).Count(typeof(T));
        if (size is null)
        {
            throw new InvalidOperationException("Cannot probe the length of unknown length receive");
        }

        received = new T[size.Value];
        comm.Receive(source, tag, ref received);
        return size.Value;
    }
}
