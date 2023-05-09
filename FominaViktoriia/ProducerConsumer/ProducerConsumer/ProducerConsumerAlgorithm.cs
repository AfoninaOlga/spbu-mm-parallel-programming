using ProducerConsumer.Locks;

namespace ProducerConsumer;

public class ProducerConsumerAlgorithm<T>
{
    private readonly List<T> buffer = new();
    private readonly List<Task> produceTasks = new();
    private readonly List<Task> consumeTasks = new();
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private readonly CancellationToken cancellationToken;
    private readonly ILock locker;
    private bool isTest;

    public ProducerConsumerAlgorithm(int producersSize, int consumersSize,
            Func<T> produce, bool isTest = false)
    {
        this.isTest = isTest;

        if (producersSize <= 0)
        {
            throw new ArgumentException("Consumers size should be > 0");
        }

        if (consumersSize <= 0)
        {
            throw new ArgumentException("Consumers size should be > 0");
        }

        cancellationToken = cancellationTokenSource.Token;

        locker = new TTASLock();

        for (var i = 0; i < producersSize; ++i)
        {
            var producer = new Producer<T>(produce, buffer, i, locker, cancellationToken);
            produceTasks.Add(new Task(producer.Produce));
        }

        for (var i = 0; i < consumersSize; ++i)
        {
            var consumer = new Consumer<T>(buffer, i, locker, cancellationToken);
            consumeTasks.Add(new Task(consumer.Consume));
        }
    }

    public void Run()
    {
        produceTasks.ForEach(x => x.Start());
        consumeTasks.ForEach(x => x.Start());

        if (isTest)
        {
            Thread.Sleep(1000);
        }
        else
        {
            Console.ReadKey();
        }
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}
