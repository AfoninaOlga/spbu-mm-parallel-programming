using System.Collections.Concurrent;

namespace ProducerConsumerTest;

public class ConsumerTest
{
    private Buffer<int> _buffer;
    
    [SetUp]
    public void Setup()
    {
        _buffer = new Buffer<int>();
    }
    [Test]
    public void NotStartedConsumerTest()
    {
        var consumer = new Consumer<int>(1, _buffer, item => { });
        Assert.Throws<ThreadStateException>(() => consumer.Stop());
    }
    [Test]
    public void ConsumerTakesAllTest()
    {
        var testItems = new[] {1, 2, 3, 4};
        foreach (var item in testItems)
        {
            _buffer.Add(item);
        }

        var consumedItems = new ConcurrentQueue<int>();
        var consumer = new Consumer<int>(1, _buffer, item => { consumedItems.Enqueue(item); });
        consumer.Start();
        Thread.Sleep(500);
        consumer.Stop();

        Assert.That(consumedItems.Count, Is.EqualTo(testItems.Length));
        foreach (var item in consumedItems)
        {
            Assert.That(testItems.Contains(item));
        }
    }
    [Test]
    public void MultipleConsumerTakeTest()
    {
        var testItems = new[] {1, 2, 3, 4};
        foreach (var item in testItems)
        {
            _buffer.Add(item);
        }

        var consumedItems = new ConcurrentQueue<int>();
        Action<int> consume = item => { consumedItems.Enqueue(item); Thread.Sleep(100); };
        var consumer1 = new Consumer<int>(1, _buffer, consume);
        var consumer2 = new Consumer<int>(2, _buffer, consume);
        consumer1.Start();
        consumer2.Start();
        Thread.Sleep(1000);
        consumer1.Stop();
        consumer2.Stop();

        Assert.That(consumedItems.Count, Is.EqualTo(testItems.Length));
        foreach (var item in consumedItems)
        {
            Assert.That(testItems.Contains(item));
        }
    }
}