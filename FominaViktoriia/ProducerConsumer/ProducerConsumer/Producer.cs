using ProducerConsumer.Locks;

namespace ProducerConsumer;

internal class Producer<T>
{
    private readonly Func<T> produce;
    private readonly int id;
    private readonly List<T> buffer;
    private readonly CancellationToken cancellationToken;
    private readonly ILock locker;

    internal Producer(Func<T> produce, List<T> buffer, int id,
            ILock locker, CancellationToken cancellationToken)
    {
        this.produce = produce;
        this.id = id;
        this.buffer = buffer;
        this.cancellationToken = cancellationToken;
        this.locker = locker;
    }

    internal void Produce()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            locker.Lock();
            var produced = produce.Invoke();
            Console.WriteLine($"Producer {id} produced a value");
            buffer.Add(produced);
            locker.Unlock();

            Thread.Sleep(1000);
        }
    }
}
