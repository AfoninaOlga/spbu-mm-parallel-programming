namespace ThreadPoolTest;

[TestFixture(WorkStrategy.Sharing)]
[TestFixture(WorkStrategy.Stealing)]
public class ThreadPoolTest
{
    private const int ThreadPoolSize = 4;
    private MyThreadPool _tp;
    private WorkStrategy _strategy;

    public ThreadPoolTest(WorkStrategy strategy)
    {
        _strategy = strategy;
    }

    [SetUp]
    public void Startup()
    {
        _tp = new MyThreadPool(ThreadPoolSize, _strategy);
    }

    [TearDown]
    public void Cleanup()
    {
        try
        {
            _tp.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Ignored for tests with disposing ThreadPool
        }
    }

    [Test]
    public void NotCompletedTest()
    {
        using var task = new MyTask<int>(() => 6 * 7);
        Assert.That(task.IsCompleted, Is.False);
    }

    [Test]
    public void CompletedTest()
    {
        using var task = new MyTask<int>(() => 6 * 7);
        _tp.Enqueue(task);
        var taskResult = task.Result;
        Assert.That(task.IsCompleted);
    }

    [Test]
    public void TaskResultTest()
    {
        using var task = new MyTask<int>(() => 6 * 7);
        _tp.Enqueue(task);
        Assert.That(task.Result, Is.EqualTo(42));
    }

    [Test]
    public void DisposedExceptionTest()
    {
        _tp.Dispose();
        Assert.Throws<ObjectDisposedException>(() => _tp.Dispose());
    }

    [Test]
    public void NullTaskTest()
    {
        Assert.Throws<ArgumentNullException>(() => _tp.Enqueue((IMyTask<int>)null));
    }

    [Test]
    public void EnqueueFailTest()
    {
        using var task = new MyTask<int>(() => ((IMyTask<int>)null).Result);
        _tp.Enqueue(task);
        var exception = Assert.Throws<AggregateException>(() =>
        {
            var taskResult = task.Result;
        });
        Assert.IsInstanceOf<NullReferenceException>(exception.InnerException);
    }

    [Test]
    public void AggregateExceptionTest()
    {
        var exceptions = new List<Exception>
        {
            new NullReferenceException(),
            new ArithmeticException(),
        };
        using var task = new MyTask<int>(() => throw new AggregateException(exceptions));
        _tp.Enqueue(task);
        var aggregateException = Assert.Throws<AggregateException>(() =>
        {
            var taskResult = task.Result;
        });
        foreach (var innerException in aggregateException!.Flatten().InnerExceptions)
        {
            Assert.That(exceptions.Contains(innerException));
        }
    }

    [Test]
    public void MoreTasksThanThreadPoolSizeTest()
    {
        const int tasksCount = 2 * ThreadPoolSize;

        var tasks = new MyTask<int>[tasksCount];
        for (int i = 0; i < tasksCount; ++i)
        {
            tasks[i] = new MyTask<int>(() =>
            {
                Thread.Sleep(500);
                return 6 * 7;
            });
        }

        foreach (var task in tasks)
        {
            _tp.Enqueue(task);
        }

        Assert.That(_tp.Size, Is.EqualTo(ThreadPoolSize));
        _tp.Dispose();
        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EqualTo(42));
            task.Dispose();
        }
    }

    [Test]
    public void AddTasksInParallelTest()
    {
        const int parallelThreadsCount = 40;
        var threads = new List<Thread>();
        for (var i = 0; i < parallelThreadsCount; ++i)
        {
            var j = i;
            var thread = new Thread(() =>
            {
                using var task = new MyTask<int>(() => j);
                _tp.Enqueue(task);
                Assert.That(task.Result, Is.EqualTo(j));
            });
            threads.Add(thread);
            thread.Start();
        }

        threads.ForEach(thread => thread.Join());
    }

    [Test]
    public void EmptyThreadPoolTest()
    {
        Assert.That(_tp.Size, Is.EqualTo(ThreadPoolSize));
    }

    [Test]
    public void LessTasksThanThreadPoolSizeTest()
    {
        const int tasksCount = ThreadPoolSize - 1;
        var tasks = new MyTask<int>[tasksCount];
        for (int i = 0; i < tasksCount; ++i)
        {
            tasks[i] = new MyTask<int>(() =>
            {
                Thread.Sleep(500);
                return 6 * 7;
            });
        }

        foreach (var task in tasks)
        {
            _tp.Enqueue(task);
        }

        Assert.That(_tp.Size, Is.EqualTo(ThreadPoolSize));
        _tp.Dispose();
        foreach (var task in tasks)
        {
            task.Dispose();
        }
    }

    [Test]
    public void FinishAfterDisposeTest()
    {
        var task = new MyTask<int>(() =>
        {
            Thread.Sleep(1000);
            return 6 * 7;
        });
        _tp.Enqueue(task);
        _tp.Dispose();
        Assert.That(task.Result, Is.EqualTo(42));
        task.Dispose();
    }

    [Test]
    public void ObjectDisposedExceptionTest()
    {
        _tp.Dispose();
        using var task = new MyTask<int>(() => 6 * 7);
        Assert.Throws<ObjectDisposedException>(() => _tp.Enqueue(task));
    }

    [Test]
    public void BaseContinuationTest()
    {
        using var taskA = new MyTask<string>(() => "A");
        using var taskB = taskA.ContinueWith(a => $"{a}B");
        _tp.Enqueue(taskA);
        _tp.Enqueue(taskB);
        Assert.That(taskA.Result, Is.EqualTo("A"));
        Assert.That(taskB.Result, Is.EqualTo("AB"));
    }

    [Test]
    public void MultipleContinuationTest()
    {
        using var taskA = new MyTask<int>(() => 1);
        using var taskB = taskA.ContinueWith(a => a + 2);
        using var taskC = taskA.ContinueWith(a => a + 3);
        _tp.Enqueue(taskA);
        _tp.Enqueue(taskB);
        _tp.Enqueue(taskC);
        Assert.Multiple(() =>
        {
            Assert.That(taskB.Result, Is.EqualTo(3));
            Assert.That(taskC.Result, Is.EqualTo(4));
        });
    }

    [Test]
    public void ContinuationWaitBaseTaskTest()
    {
        using var taskA = new MyTask<int>(() => 1);
        using var taskB = taskA.ContinueWith(a => a + 2);
        _tp.Enqueue(taskB);
        Thread.Sleep(500);
        Assert.IsFalse(taskB.IsCompleted);
        _tp.Enqueue(taskA);
        _tp.Dispose();
    }
}

[TestFixture(WorkStrategy.Sharing, 42, 10)]
[TestFixture(WorkStrategy.Stealing, 42, 10)]
[TestFixture(WorkStrategy.Sharing, 10, 42)]
[TestFixture(WorkStrategy.Stealing, 10, 42)]
public class HugeThreadPoolTest
{
    private MyThreadPool _tp;
    private int _taskCount;

    public HugeThreadPoolTest(WorkStrategy strategy, int threadPoolSize, int taskCount)
    {
        _tp = new MyThreadPool(threadPoolSize, strategy);
        _taskCount = taskCount;
    }

    [TearDown]
    public void Cleanup()
    {
        try
        {
            _tp.Dispose();
        }
        catch (ObjectDisposedException)
        {
            // Ignored for tests with disposing ThreadPool
        }
    }
    [Test]
    public void RunLotsOfThreadsTest()
    {
        const int tpSize = 42;
        using var tp = new MyThreadPool(tpSize);
        var tasks = new List<IMyTask<int>>();
        for (var i = 0; i < _taskCount; ++i)
        {
            var task = new MyTask<int>(() =>
            {
                Thread.Sleep(100);
                return 6 * 7;
            });
            tp.Enqueue(task);
            tasks.Add(task);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EqualTo(42));
            task.Dispose();
        }
    }
}