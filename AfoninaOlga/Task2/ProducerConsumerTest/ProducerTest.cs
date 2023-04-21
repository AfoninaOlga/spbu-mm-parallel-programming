namespace ProducerConsumerTest;

public class ProducerTest
{
    private Buffer<int> _buffer;
    
    [SetUp]
    public void Setup()
    {
        _buffer = new Buffer<int>();
    }

    [Test]
    public void NotStartedProducerTest()
    {
        var producer = new Producer<int>(1, _buffer, () => 1);
        Assert.Throws<ThreadStateException>(() => producer.Stop());
    }

    [Test]
    public void ProducerAddTest()
    {
        var producer = new Producer<int>(1, _buffer, () => 1);
        producer.Start();
        Thread.Sleep(100);
        producer.Stop();
        Assert.Multiple(() =>
        {
            Assert.That(_buffer.Take(out var item));
            Assert.That(item, Is.EqualTo(1));
        });
    }

    [Test]
    public void MultipleProducerAddTest()
    {
        var testItems = new[] { 1, 2 };
        var producer1 = new Producer<int>(1, _buffer, () => testItems[0]);
        var producer2 = new Producer<int>(2, _buffer, () => testItems[1]);
        producer1.Start();
        producer2.Start();
        Thread.Sleep(100);
        producer1.Stop();
        producer2.Stop();

        var producedStrings = new List<int>();
        while (_buffer.Take(out var item))
        {
            producedStrings.Add(item);
        }

        foreach (var item in testItems)
        {
            Assert.That(producedStrings.Contains(item));
        }
    }
}