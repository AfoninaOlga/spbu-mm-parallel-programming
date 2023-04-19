namespace ProducerConsumer;

public abstract class Worker<T>
{
    protected int Id { get; }
    private readonly Thread _thread;
    private volatile bool _isActive;
    protected Buffer<T> Buffer { get; }

    protected abstract void Work();

    protected Worker(int id, Buffer<T> buffer)
    {
        Id = id;
        Buffer = buffer;
        _thread = new Thread(Run);
    }
    
    public void Start()
    {
        _isActive = true;
        _thread.Start();
    }
    public void Stop()
    {
        _isActive = false;
        _thread.Join();
    }

    private void Run()
    {
        while (_isActive)
        {
            Work();
        }
    }
}