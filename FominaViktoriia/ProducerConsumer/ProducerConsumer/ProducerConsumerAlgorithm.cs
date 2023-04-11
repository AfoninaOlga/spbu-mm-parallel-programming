namespace ProducerConsumer;

internal class ProducerConsumerAlgorithm<T>
{
    private readonly List<T> buffer = new();
    private readonly List<Task> produceTasks = new();
    private readonly List<Task> consumeTasks = new();
    private readonly CancellationToken cancellationToken = new();

    internal ProducerConsumerAlgorithm(int producersSize, int consumersSize,
            Func<T> produce)
    {
        if (producersSize <= 0)
        {
            throw new Exception("Consumers size should be > 0");
        }

        if (consumersSize <= 0)
        {
            throw new Exception("Consumers size should be > 0");
        }

        for (var i = 0; i < producersSize; ++i)
        {
            var producer = new Producer<T>(produce, buffer, i, cancellationToken);
            produceTasks.Add(new Task(producer.Produce));
        }

        for (var i = 0; i < consumersSize; ++i)
        {
            var consumer = new Consumer<T>(buffer, i, cancellationToken);
            consumeTasks.Add(new Task(consumer.Consume));
        }
    }

    internal void Run()
    {
        produceTasks.ForEach(x => x.Start());
        consumeTasks.ForEach(x => x.Start());
    }
}
