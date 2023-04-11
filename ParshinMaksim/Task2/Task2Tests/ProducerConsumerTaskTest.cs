using Task2;

namespace Task2Tests;

public class ProducerConsumerTaskTests
{
    [Test, Timeout(2000)]
    public void SmokeTest()
    {
        var producerToken = new CancellationTokenSource();
        var consumerToken = new CancellationTokenSource();

        var expected = 42;

        int Produce()
        {
            producerToken.Cancel();
            return expected;
        }

        var consumed = 0;

        void Consume(int value)
        {
            consumed = value;
            consumerToken.Cancel();
        }

        var task = new ProducerConsumerTask<int>();
        var producer = task.AddProducer(Produce);
        var consumer = task.AddConsumer(Consume);

        var producerTask = producer.Start(producerToken.Token);
        var consumerTask = consumer.Start(consumerToken.Token);

        producerTask.Wait();
        consumerTask.Wait();

        Assert.That(consumed, Is.EqualTo(expected));
    }

    [Test, Timeout(3000)]
    public void AllIsConsumed()
    {
        var producerToken = new CancellationTokenSource();
        var consumerToken = new CancellationTokenSource(2000);

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
        var producer = task.AddProducer(Produce, CanProduce);
        var consumer = task.AddConsumer(Consume);

        var producerTask = producer.Start(producerToken.Token);
        var consumerTask = consumer.Start(consumerToken.Token);

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
        var consumers = new List<IConsumer>();
        var producers = new List<IProducer>();

        for (var i = 0; i < producersCount; ++i)
        {
            var id = i;

            int Produce()
            {
                return id;
            }

            var producer = task.AddProducer(Produce);
            producers.Add(producer);
        }

        for (var i = 0; i < consumersCount; ++i)
        {
            void Consume(int value) { }

            var consumer = task.AddConsumer(Consume);
            consumers.Add(consumer);
        }

        var token = new CancellationTokenSource(2000);

        var consumersTasks = consumers.Select(c => c.Start(token.Token)).ToArray();
        var producersTasks = producers.Select(c => c.Start(token.Token)).ToArray();

        Task.WaitAll(consumersTasks);
        Task.WaitAll(producersTasks);

        Assert.Pass();
    }
}
