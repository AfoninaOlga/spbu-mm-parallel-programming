namespace ProducerConsumer;

public class Buffer<T>
{
    private readonly List<T> _data = new();
    private readonly Semaphore _semaphore = new(1, 1);

    public void Add(T item)
    {
        _semaphore.WaitOne();
        _data.Add(item);
        _semaphore.Release();
    }

    public bool Take(out T? item)
    {
        _semaphore.WaitOne();

        if (GetSize() > 0)
        {
            item = _data[0];
            _data.RemoveAt(0);
            _semaphore.Release();
            return true;
        }

        item = default;
        _semaphore.Release();
        return false;
    }

    public int GetSize()
    {
        return _data.Count;
    }
}