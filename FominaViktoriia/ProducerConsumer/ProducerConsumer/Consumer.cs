namespace ProducerConsumer;

internal class Consumer<T>
{
    private readonly List<T> buffer;
    private readonly int id;
    private readonly CancellationToken cancellationToken;

    internal Consumer(List<T> buffer, int id, CancellationToken cancellationToken)
    {
        this.buffer = buffer;
        this.id = id;
        this.cancellationToken = cancellationToken;
    }

    internal void Consume()
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // ToDo: add syncronization.
            if (buffer.Count > 0)
            {
                buffer.RemoveAt(0);
            }
            Console.WriteLine($"Consumer {id} consumed a value");
        }
    }
}
