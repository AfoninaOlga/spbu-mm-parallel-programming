namespace ProducerConsumer;

internal class Producer<T>
{
    private readonly Func<T> produce;
    private readonly int id;
    private readonly List<T> buffer;
    private CancellationToken cancellationToken;

    internal Producer(Func<T> produce, List<T> buffer, int id,
            CancellationToken cancellationToken)
    {
        this.produce = produce;
        this.id = id;
        this.buffer = buffer;
        this.cancellationToken = cancellationToken;
    }

    internal void Produce()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // ToDo: add syncronization.
            var produced = produce.Invoke();
            Console.WriteLine($"Producer {id} produced a value");
            buffer.Add(produced);
            Thread.Sleep(1000);
        }
    }
}
