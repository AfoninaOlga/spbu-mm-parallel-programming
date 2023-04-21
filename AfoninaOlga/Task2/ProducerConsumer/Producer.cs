namespace ProducerConsumer;
public class Producer<T>: Worker<T>
{
    private readonly Func<T> _produceData;
    public Producer(int id, Buffer<T> buffer, Func<T> produceData) : base(id, buffer)
    {
        _produceData = produceData;
        Console.WriteLine($"Producer #{Id} created");
    }

    protected override void Work()
    {
        Buffer.Add(_produceData.Invoke());
    }
}