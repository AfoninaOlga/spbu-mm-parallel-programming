using Task2;

namespace Task2Tests;

public class ProducerConsumerTaskTests
{
    [Test, Timeout(2000)]
    public void SmokeTest()
    {
        var producerTokenSrc = new CancellationTokenSource();
        var consumerTokenSrc = new CancellationTokenSource();

        var expected = 42;

        int Produce()
        {
            producerTokenSrc.Cancel();
            return expected;
        }

        var consumed = 0;

        void Consume(int value)
        {
            consumed = value;
            consumerTokenSrc.Cancel();
        }

        var task = new ProducerConsumerTask<int>();
        var producer = new Producer<int>(Produce);
        var consumer =  new Consumer<int>(Consume);

        var producerTask = task.AddProducer(producer, producerTokenSrc.Token);
        var consumerTask = task.AddConsumer(consumer, consumerTokenSrc.Token);

        producerTask.Start();
        consumerTask.Start();

        producerTask.Wait();
        consumerTask.Wait();

        Assert.That(consumed, Is.EqualTo(expected));
    }

    [Test, Timeout(3000)]
    public void AllIsConsumed()
    {
        var producerTokenSrc = new CancellationTokenSource();
        var consumerTokenSrc = new CancellationTokenSource(2000);

        var toProduce = new [] { 1, 2, 3, 4, 5 };
        var currentIndex = 0;

        var consumed = new HashSet<int>();

        int Produce()
        {
            var value = toProduce[currentIndex];
            currentIndex++;
            return value;
        }

        bool CanProduce() => currentIndex < toProduce.Length;

        void Consume(int value)
        {
            consumed.Add(value);
        }

        var task = new ProducerConsumerTask<int>();
        var producer = new Producer<int>(Produce, CanProduce);
        var consumer = new Consumer<int>(Consume);

        var producerTask = task.AddProducer(producer, producerTokenSrc.Token);
        var consumerTask = task.AddConsumer(consumer, consumerTokenSrc.Token);

        producerTask.Start();
        consumerTask.Start();

        producerTask.Wait();
        consumerTask.Wait();

        Assert.That(consumed, Is.EquivalentTo(toProduce));
    }

    [Test, Timeout(10000)]
    [TestCase(5, 5)]
    [TestCase(0, 5)]
    [TestCase(5, 0)]
    [TestCase(50, 50)]
    public void MultipleActors(int producersCount, int consumersCount)
    {
        var task = new ProducerConsumerTask<int>();
        var consumerTasks = new Task[consumersCount];
        var producerTasks = new Task[producersCount];
        var tokenSource = new CancellationTokenSource(2000);

        for (var i = 0; i < producersCount; ++i)
        {
            var id = i;
            var producer = new Producer<int>(() => id);
            var producerTask = task.AddProducer(producer, tokenSource.Token);
            producerTask.Start();
            producerTasks[i] = producerTask;
        }

        for (var i = 0; i < consumersCount; ++i)
        {
            var consumer = new Consumer<int>(_ => { });
            var consumerTask = task.AddConsumer(consumer, tokenSource.Token);
            consumerTask.Start();
            consumerTasks[i] = consumerTask;
        }

        Task.WaitAll(consumerTasks);
        Task.WaitAll(producerTasks);

        Assert.Pass();
    }
}
