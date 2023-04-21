namespace ProducerConsumerTest;

public class BufferTests
{
    private Buffer<int> _buffer;
    [SetUp]
    public void Setup()
    {
        _buffer = new Buffer<int>();
    }

    [Test]
    public void EmptyTest()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_buffer.Take(out var item), Is.False);
            Assert.That(_buffer.GetSize(), Is.EqualTo(0));
        });
    }

    [Test]
    public void AddTest()
    {
        for (var i = 1; i < 5; ++i)
        {
            _buffer.Add(i);
            Assert.That(_buffer.GetSize(), Is.EqualTo(i));
        }
    }

    [Test]
    public void TakeTest()
    {
        for (var i = 1; i < 5; ++i)
        {
            _buffer.Add(i);
        }
        for (var i = 1; i < 5; ++i)
        {
            var tookItem = _buffer.Take(out var item);
            Assert.Multiple(() =>
            {
                Assert.That(tookItem);
                Assert.That(item, Is.EqualTo(i));
                Assert.That(_buffer.GetSize(), Is.EqualTo(4 - i));
            });
        }
    }
}