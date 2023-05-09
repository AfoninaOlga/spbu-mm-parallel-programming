using ProducerConsumer.Locks;

namespace ProducerConsumer;

internal class Consumer<T>
{
    private readonly List<T> buffer;
    private readonly int id;
    private readonly CancellationToken cancellationToken;
    private readonly ILock locker;

    internal Consumer(List<T> buffer, int id, ILock locker, CancellationToken cancellationToken)
    {
        this.buffer = buffer;
        this.id = id;
        this.cancellationToken = cancellationToken;
        this.locker = locker;
    }

    internal void Consume()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            locker.Lock();
            if (buffer.Count > 0)
            {
                buffer.RemoveAt(0);
            }
            Console.WriteLine($"Consumer {id} consumed a value");
            locker.Unlock();

            Thread.Sleep(1000);
        }
    }
}
