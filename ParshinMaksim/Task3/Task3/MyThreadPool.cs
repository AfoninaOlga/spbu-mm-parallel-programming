using System;
using System.Threading;
using System.Collections.Generic;

namespace Task3;

public class MyThreadPool : IMyThreadPool
{
	private const int WorkSharingThreshold = 5;

	private readonly Thread[] threads;
	private readonly Queue<Action>[] actionQueues;
	private readonly object[] actionQueueLockers;
	private readonly AutoResetEvent[] actionQueueEvents;
	private readonly ThreadPoolStrategy strategy;
	private readonly CancellationTokenSource cancellationTokenSource = new();
	private readonly Random random = new();

	private int currentIndexToEnqueue;
	private readonly object currentIndexToEnqueueLocker = new();

	/// <summary>
	/// Cancellation token of the pool. Cancelled when Shutdown is called
	/// </summary>
	public CancellationToken CancellationToken => cancellationTokenSource.Token;

	public MyThreadPool(int threadCount, ThreadPoolStrategy strategy)
	{
		if (threadCount <= 0)
		{
			throw new ArgumentOutOfRangeException(
				nameof(threadCount),
				"Thread number must be greater than zero");
		}

		threads = new Thread[threadCount];
		actionQueues = new Queue<Action>[threadCount];
		actionQueueLockers = new object[threadCount];
		actionQueueEvents = new AutoResetEvent[threadCount];
		this.strategy = strategy;

		for (var i = 0; i < threadCount; ++i)
		{
			var threadIndex = i;
			actionQueues[i] = new Queue<Action>();
			actionQueueLockers[i] = new object();
			actionQueueEvents[i] = new AutoResetEvent(false);

			void DoTasks()
			{
				switch (this.strategy)
				{
					case ThreadPoolStrategy.WorkSharing:
					{
						DoTasksSharing(threadIndex);
						break;
					}

					case ThreadPoolStrategy.WorkStealing:
					{
						DoTasksStealing(threadIndex);
						break;
					}

					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			threads[i] = new Thread(DoTasks);
		}

		for (var i = 0; i < threadCount; ++i)
		{
			threads[i].Start();
		}
	}

	private void DoTasksStealing(int myIndex)
	{
		while (!cancellationTokenSource.IsCancellationRequested)
		{
			Action? nextAction;

			lock (actionQueueLockers[myIndex])
			{
				actionQueues[myIndex].TryDequeue(out nextAction);
			}

			if (nextAction is not null)
			{
				nextAction.Invoke();
				continue;
			}

			var randomOffset = random.Next(0, actionQueues.Length);

			for (var i = 0; i < actionQueues.Length && nextAction is null; ++i)
			{
				var randomizedIndex = (i + randomOffset) % actionQueues.Length;

				if (randomizedIndex == myIndex)
				{
					continue;
				}

				lock (actionQueueLockers[randomizedIndex])
				{
					actionQueues[randomizedIndex].TryDequeue(out nextAction);
				}
			}

			if (nextAction is not null)
			{
				nextAction.Invoke();
				continue;
			}

			actionQueueEvents[myIndex].WaitOne();
		}
	}

	private void BalanceQueues(int index1, int index2)
	{
		var minIndex = index1;
		var maxIndex = index2;

		if (index1 > index2)
		{
			minIndex = index2;
			maxIndex = index1;
		}

		var queueWithMinIndex = actionQueues[minIndex];
		var queueWithMaxIndex = actionQueues[maxIndex];

		lock (queueWithMinIndex)
		{
			lock (queueWithMaxIndex)
			{
				var queueWithLessElements = queueWithMinIndex;
				var queueWithMoreElements = queueWithMaxIndex;
				var queueToEnqueueIndex = minIndex;

				if (queueWithMinIndex.Count > queueWithMaxIndex.Count)
				{
					queueWithLessElements = queueWithMaxIndex;
					queueWithMoreElements = queueWithMinIndex;
					queueToEnqueueIndex = maxIndex;
				}

				var diff = queueWithMoreElements.Count - queueWithLessElements.Count;

				if (diff <= WorkSharingThreshold)
				{
					return;
				}

				while (queueWithMoreElements.Count > queueWithLessElements.Count)
				{
					queueWithLessElements.Enqueue(queueWithMoreElements.Dequeue());

				}

				actionQueueEvents[queueToEnqueueIndex].Set();
			}
		}
	}

	private void DoTasksSharing(int myIndex)
	{
		while (!cancellationTokenSource.IsCancellationRequested)
		{
			Action? nextAction;
			int queueCount;

			lock (actionQueueLockers[myIndex])
			{
				actionQueues[myIndex].TryDequeue(out nextAction);
				queueCount = actionQueues[myIndex].Count;
			}

			nextAction?.Invoke();

			var shouldDoBalance = random.Next(0, queueCount + 1) == queueCount;

			if (shouldDoBalance)
			{
				var victimIndex = random.Next(0, actionQueues.Length);

				if (victimIndex != myIndex)
				{
					BalanceQueues(victimIndex, myIndex);
				}
			}

			if (nextAction is null)
			{
				actionQueueEvents[myIndex].WaitOne();
			}
		}
	}

	/// <summary>
	/// Enqueues an action to the thread pool
	/// </summary>
	/// <param name="supplier">Action to enqueue</param>
	public void Enqueue(Action supplier)
	{
		lock (currentIndexToEnqueueLocker)
		{
			lock (actionQueueLockers[currentIndexToEnqueue])
			{
				actionQueues[currentIndexToEnqueue].Enqueue(supplier);
			}

			actionQueueEvents[currentIndexToEnqueue].Set();

			currentIndexToEnqueue = (currentIndexToEnqueue + 1) % actionQueues.Length;
		}
	}

	/// <summary>
	/// Creates a new task which result is evaluated by the thread pool
	/// </summary>
	/// <typeparam name="TResult">Task result type</typeparam>
	/// <param name="supplier">Function to be encapsulated into the new task</param>
	/// <returns>Created task</returns>
	public IMyTask<TResult> Enqueue<TResult>(Func<TResult> supplier)
	{
		var task = new MyTask<TResult>(supplier, this);
		task.Run();
		return task;
	}

	/// <summary>
	/// Stops the thread pool work. If there are queued tasks left, they throw ThreadPoolShutdownException to the waiting threads
	/// </summary>
	public void Shutdown()
	{
		if (cancellationTokenSource.IsCancellationRequested)
		{
			return;
		}

		cancellationTokenSource.Cancel();

		foreach (var queueEvent in actionQueueEvents)
		{
			queueEvent.Set();
		}
	}

	public void Dispose()
	{
		Shutdown();
		foreach (var thread in threads)
		{
			thread.Join();
		}
		cancellationTokenSource.Dispose();
	}
}
