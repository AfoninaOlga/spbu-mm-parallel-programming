namespace ProducerConsumer;

public class Consumer<T>: Worker<T>
{
    private readonly Action<T> _consumeData;
    
    public Consumer(int id, Buffer<T> buffer, Action<T> consumeData) : base(id, buffer)
    {
        _consumeData = consumeData;
        Console.WriteLine($"Consumer #{Id} created");
    }

    protected override void Work()
    {
        var tookItem = Buffer.Take(out var item);
        if (tookItem && item != null)
        {
            _consumeData.Invoke(item);
        }
    }
}