using System.Collections.Concurrent;
using Task3;

namespace Task3Tests;

[TestFixture]
public class MyThreadPoolTests
{
	[Test, Combinatorial, Repeat(3)]
	public void ThreadCountTest(
		[Values(1, 3, 5, 10, 20)] int threadCount,
		[Values(ThreadPoolStrategy.WorkSharing, ThreadPoolStrategy.WorkStealing)] ThreadPoolStrategy strategy)
	{
		using var pool = new MyThreadPool(threadCount, strategy);
		var idBag = new ConcurrentBag<int>();
		var handles = new WaitHandle[threadCount];

		for (var i = 0; i < threadCount; ++i)
		{
			handles[i] = new AutoResetEvent(false);
			var localIndex = i;

			pool.Enqueue(() =>
			{
				idBag.Add(Environment.CurrentManagedThreadId);
				Thread.Sleep(1000);
				var resetEvent = (AutoResetEvent)handles[localIndex];
				resetEvent.Set();
				return 0;
			});
		}

		WaitHandle.WaitAll(handles);
		Assert.That(idBag.Distinct().Count(), Is.EqualTo(idBag.Count));
		pool.Shutdown();
	}

	[Test, Combinatorial, Repeat(3)]
	public void OneThreadManyTasksTest(
		[Values(1, 3, 5, 10)] int taskCount,
		[Values(ThreadPoolStrategy.WorkSharing, ThreadPoolStrategy.WorkStealing)] ThreadPoolStrategy strategy)
	{
		using var pool = new MyThreadPool(1, strategy);
		var tasks = new List<IMyTask<int>>();
		var idBag = new ConcurrentBag<int>();

		for (var i = 0; i < taskCount; ++i)
		{
			var localIndex = i;

			tasks.Add(pool.Enqueue(() =>
			{
				idBag.Add(Environment.CurrentManagedThreadId);
				Thread.Sleep(1000);
				return localIndex;
			}));
		}

		for (var i = 0; i < taskCount; ++i)
		{
			Assert.That(tasks[i].Result, Is.EqualTo(i));
		}

		Assert.That(idBag.Distinct().Count(), Is.EqualTo(1));
	}

	[Test, Combinatorial, Repeat(3)]
	public void SimpleConcurrentCalculationTest(
		[Values(1, 3, 5, 10, 20)] int threadCount,
		[Values(ThreadPoolStrategy.WorkSharing, ThreadPoolStrategy.WorkStealing)] ThreadPoolStrategy strategy)
	{
		using var pool = new MyThreadPool(threadCount, strategy);
		var tasks = new List<IMyTask<int>>();

		for (var i = 0; i < threadCount; ++i)
		{
			var localIndex = i;

			tasks.Add(pool.Enqueue(() =>
			{
				Thread.Sleep(1000);
				return localIndex;
			}));
		}

		for (var i = 0; i < threadCount; ++i)
		{
			Assert.That(tasks[i].Result, Is.EqualTo(i));
		}
	}

	[Test, Combinatorial, Repeat(3)]
	public void IsCompletedTest(
		[Values(1, 3, 5, 10)] int threadCount,
		[Values(ThreadPoolStrategy.WorkSharing, ThreadPoolStrategy.WorkStealing)] ThreadPoolStrategy strategy)
	{
		using var pool = new MyThreadPool(threadCount, strategy);
		var tasks = new List<IMyTask<int>>();
		var resetEvent = new ManualResetEvent(false);

		for (var i = 0; i < threadCount; ++i)
		{
			var localIndex = i;

			tasks.Add(pool.Enqueue(() =>
			{
				resetEvent.WaitOne();
				return 0;
			}));
		}

		foreach (var task in tasks)
		{
			Assert.That(task.IsCompleted, Is.False);
		}

		resetEvent.Set();

		foreach (var task in tasks)
		{
			Assert.That(task.Result, Is.EqualTo(0));
			Assert.That(task.IsCompleted, Is.True);
		}
	}

	[Test, Combinatorial, Repeat(3)]
	public void ConcurrentResultTest(
		[Values(1, 3, 5, 10)] int threadCount,
		[Values(ThreadPoolStrategy.WorkSharing, ThreadPoolStrategy.WorkStealing)] ThreadPoolStrategy strategy)
	{
		var tasks = new Task<int>[threadCount];
		using var pool = new MyThreadPool(1, strategy);
		var resetEvent = new ManualResetEvent(false);

		var myTask = pool.Enqueue(() =>
		{
			Thread.Sleep(1000);
			return 0;
		});

		for (var i = 0; i < tasks.Length; ++i)
		{
			tasks[i] = new Task<int>(() =>
			{
				resetEvent.WaitOne();
				return myTask.Result;
			});

			tasks[i].Start();
		}

		resetEvent.Set();

		foreach (var current in tasks)
		{
			Assert.That(current.Result, Is.EqualTo(0));
		}
	}

	[Test, Combinatorial, Repeat(3)]
	public void ContinueWithShutdownTest(
		[Values(1, 3, 5, 10)] int threadCount,
		[Values(ThreadPoolStrategy.WorkSharing, ThreadPoolStrategy.WorkStealing)] ThreadPoolStrategy strategy)
	{
		using var pool = new MyThreadPool(threadCount, strategy);
		var tasks = new IMyTask<int>[threadCount + 10];
		var resetEvent = new ManualResetEvent(false);

		tasks[0] = pool.Enqueue(() =>
		{
			resetEvent.WaitOne();
			return 1;
		});

		for (var i = 1; i < tasks.Length; ++i)
		{
			tasks[i] = tasks[i - 1].ContinueWith(j => ++j);
		}

		pool.Shutdown();
		resetEvent.Set();

		for (var i = 1; i < tasks.Length; ++i)
		{
			Assert.Throws<ThreadPoolShutdownException>(() => _ = tasks[i].Result);
			Assert.That(tasks[i].IsCompleted, Is.False);
		}
	}

	[Test, Combinatorial]
	public void ContinueWithTest(
		[Values(1, 3, 5, 10)] int threadCount,
		[Values(5, 10, 20)] int taskCount,
		[Values(ThreadPoolStrategy.WorkSharing, ThreadPoolStrategy.WorkStealing)] ThreadPoolStrategy strategy)
	{
		using var pool = new MyThreadPool(threadCount, strategy);
		var tasks = new IMyTask<int>[taskCount];
		var counters = new int[taskCount];
		var resetEvent = new ManualResetEvent(false);

		tasks[0] = pool.Enqueue(() =>
		{
			resetEvent.WaitOne();
			Interlocked.Increment(ref counters[0]);
			return 1;
		});

		for (var i = 1; i < taskCount; ++i)
		{
			var localIndex = i;

			tasks[i] = tasks[i - 1].ContinueWith((j) =>
			{
				resetEvent.WaitOne();
				Interlocked.Increment(ref counters[localIndex]);
				return j * 2;
			});
		}

		resetEvent.Set();

		for (var i = 0; i < taskCount; ++i)
		{
			Assert.Multiple(() =>
			{
				Assert.That(Math.Pow(2, i), Is.EqualTo(tasks[i].Result));
				Assert.That(counters[i], Is.EqualTo(1));
			});
		}
	}

	[Test, Combinatorial, Repeat(3)]
	public void MultipleContinueWithTest(
		[Values(ThreadPoolStrategy.WorkSharing, ThreadPoolStrategy.WorkStealing)] ThreadPoolStrategy strategy)
	{
		using var pool = new MyThreadPool(3, strategy);
		var resetEvent1 = new ManualResetEvent(false);
		var resetEvent2 = new ManualResetEvent(false);

		var firstTask = pool.Enqueue(() =>
		{
			resetEvent1.WaitOne();
			return 100;
		});

		var intTask1 = firstTask.ContinueWith((j) =>
		{
			resetEvent2.WaitOne();
			return j + 100;
		});

		var intTask2 = firstTask.ContinueWith((j) =>
		{
			resetEvent2.WaitOne();
			return j + 200;
		});

		var stringTask = firstTask.ContinueWith((j) =>
		{
			resetEvent2.WaitOne();
			return j.ToString();
		});

		var boolTask = firstTask.ContinueWith((j) =>
		{
			resetEvent2.WaitOne();
			return j > 0;
		});

		resetEvent1.Set();
		Assert.Multiple(() =>
		{
			Assert.That(firstTask.Result, Is.EqualTo(100));
			Assert.That(intTask1.IsCompleted, Is.False);
			Assert.That(intTask2.IsCompleted, Is.False);
			Assert.That(stringTask.IsCompleted, Is.False);
			Assert.That(boolTask.IsCompleted, Is.False);
		});
		resetEvent2.Set();

		Assert.Multiple(() =>
		{
			Assert.That(intTask1.Result, Is.EqualTo(200));
			Assert.That(intTask2.Result, Is.EqualTo(300));
			Assert.That(stringTask.Result, Is.EqualTo("100"));
			Assert.That(boolTask.Result, Is.EqualTo(true));
			Assert.That(intTask1.IsCompleted, Is.True);
			Assert.That(intTask2.IsCompleted, Is.True);
			Assert.That(stringTask.IsCompleted, Is.True);
			Assert.That(boolTask.IsCompleted, Is.True);
		});
	}

	[Test, Combinatorial]
	public void AggregateExceptionTest(
		[Values(ThreadPoolStrategy.WorkSharing, ThreadPoolStrategy.WorkStealing)] ThreadPoolStrategy strategy)
	{
		using var pool = new MyThreadPool(3, strategy);
		var task1 = pool.Enqueue(() => 0);
		var task2 = task1.ContinueWith((j) => 1 / j);
		var task3 = task2.ContinueWith((j) => j.ToString());

		Assert.Throws<AggregateException>(() => _ = task2.Result);
		Assert.Throws<AggregateException>(() => _ = task3.Result);
	}
}
