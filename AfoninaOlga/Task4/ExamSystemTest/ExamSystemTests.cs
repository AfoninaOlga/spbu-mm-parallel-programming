using ExamSystem.Systems;

namespace ExamSystemTest;

[TestFixture(SystemType.FineGrained)]
[TestFixture(SystemType.Lazy)]
public class SequentialExamSystemTest
{
    private IExamSystem _system;
    private readonly SystemType _systemType;

    public SequentialExamSystemTest(SystemType systemType)
    {
        _systemType = systemType;
    }

    [SetUp]
    public void Startup()
    {
        if (_systemType == SystemType.FineGrained)
        {
            _system = new FineGrainedSystem();
        }
        else
        {
            _system = new LazySystem();
        }
    }

    [Test]
    public void EmptyTest()
    {
        Assert.That(_system.Count, Is.EqualTo(0));
    }

    [Test]
    public void ContainsTest()
    {
        _system.Add(1, 1);
        Assert.Multiple(() =>
        {
            Assert.That(_system.Contains(1, 1), Is.True);
            Assert.That(_system.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void RemoveAddedTest()
    {
        _system.Add(1, 1);
        _system.Remove(1, 1);
        Assert.Multiple(() =>
        {
            Assert.That(_system.Contains(1, 1), Is.False);
            Assert.That(_system.Count, Is.EqualTo(0));
        });
    }

    [Test]
    public void ParallelAddTest()
    {
    }
}

[TestFixture(SystemType.FineGrained, 4)]
[TestFixture(SystemType.FineGrained, 42)]
[TestFixture(SystemType.Lazy, 4)]
[TestFixture(SystemType.Lazy, 42)]
public class ParallelExamSystemTest
{
    private IExamSystem _system;
    private readonly SystemType _systemType;
    private readonly int _threadCount;

    public ParallelExamSystemTest(SystemType systemType, int threadCount)
    {
        _systemType = systemType;
        _threadCount = threadCount;
    }

    [SetUp]
    public void Startup()
    {
        if (_systemType == SystemType.FineGrained)
        {
            _system = new FineGrainedSystem();
        }
        else
        {
            _system = new LazySystem();
        }
    }

    [Test]
    public void AddTest()
    {
        var threads = new Thread[_threadCount];
        for (var i = 0; i < _threadCount; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() => _system.Add(localI, localI));
            threads[i].Start();
        }

        for (var i = 0; i < _threadCount; ++i)
        {
            threads[i].Join();
        }

        for (var i = 0; i < _threadCount; ++i)
        {
            Assert.That(_system.Contains(i, i), Is.True);
        }

        Assert.That(_system.Count, Is.EqualTo(_threadCount));
    }

    [Test]
    public void RemoveAddedTest()
    {
        var addingThreads = new Thread[_threadCount];
        var removingThreads = new Thread[_threadCount];
        for (var i = 0; i < _threadCount; ++i)
        {
            var localI = i;
            addingThreads[i] = new Thread(() => _system.Add(localI, localI));
            addingThreads[i].Start();
            removingThreads[i] = new Thread(() => _system.Remove(localI, localI));
            removingThreads[i].Start();
        }

        for (var i = 0; i < _threadCount; ++i)
        {
            addingThreads[i].Join();
            removingThreads[i].Join();
        }

        for (var i = 0; i < _threadCount; ++i)
        {
            Assert.That(_system.Contains(i, i), Is.False);
        }

        Assert.That(_system.Count, Is.EqualTo(0));
    }

    [Test]
    public void RemoveNotAddedTest()
    {
        var addingThreads = new Thread[_threadCount];
        var removingThreads = new Thread[_threadCount];
        for (var i = 0; i < _threadCount; ++i)
        {
            var localI = i;
            addingThreads[i] = new Thread(() => _system.Add(localI, localI));
            addingThreads[i].Start();
            removingThreads[i] = new Thread(() => _system.Remove(localI, localI + 1));
            removingThreads[i].Start();
        }

        for (var i = 0; i < _threadCount; ++i)
        {
            Assert.That(_system.Contains(i, i), Is.True);
        }

        Assert.That(_system.Count, Is.EqualTo(_threadCount));
    }

    [Test]
    public void AddMultipleTimesTest()
    {
        var addingThreads = new Thread[_threadCount];
        for (var i = 0; i < _threadCount; ++i)
        {
            addingThreads[i] = new Thread(() => _system.Add(1, 1));
            addingThreads[i].Start();
        }

        for (var i = 0; i < _threadCount; ++i)
        {
            addingThreads[i].Join();
        }

        Assert.Multiple(() =>
        {
            Assert.That(_system.Contains(1, 1), Is.True);
            Assert.That(_system.Count, Is.EqualTo(1));
        });
    }

    [Test]
    public void AddMultipleTimesAndRemoveTest()
    {
        var addingThreads = new Thread[_threadCount];
        for (var i = 0; i < _threadCount; ++i)
        {
            addingThreads[i] = new Thread(() => _system.Add(1, 1));
            addingThreads[i].Start();
        }

        for (var i = 0; i < _threadCount; ++i)
        {
            addingThreads[i].Join();
        }

        _system.Remove(1, 1);

        Assert.Multiple(() =>
        {
            Assert.That(_system.Contains(1, 1), Is.False);
            Assert.That(_system.Count, Is.EqualTo(0));
        });
    }
}