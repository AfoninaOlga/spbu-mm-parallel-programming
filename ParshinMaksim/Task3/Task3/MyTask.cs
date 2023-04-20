namespace Task3;

public class MyTask<TResult> : IMyTask<TResult>
{
	private readonly Queue<Action> continuations = new();
	private readonly IMyThreadPool threadPool;
	private Func<TResult>? supplier;
	private TResult? result;
	private AggregateException? aggregateException;
	private readonly ManualResetEvent isResultReadyEvent = new(false);
	private readonly Object continuationQueueLocker = new();

	/// <summary>
	/// Returns true if the task was finished successfully or with exception
	/// </summary>
	public bool IsCompleted { get; private set; }

	/// <summary>
	/// Returns result of the task. If it has not been calculated, the current thread waits for it
	/// </summary>
	/// <exception cref="ThreadPoolShutdownException">Thrown if the thread pool was shutdown and result was not evaluated</exception>
	/// <exception cref="AggregateException">Thrown if another exception was thrown during task evaluation</exception>
	public TResult Result
	{
		get
		{
			var handles = new[] { threadPool.CancellationToken.WaitHandle, isResultReadyEvent };
			WaitHandle.WaitAny(handles);

			if (!IsCompleted && threadPool.CancellationToken.IsCancellationRequested)
			{
				throw new ThreadPoolShutdownException();
			}

			if (aggregateException != null)
			{
				throw aggregateException;
			}

			return result;
		}
	}

	private void StartTask()
	{
		try
		{
			result = supplier();
		}
		catch (Exception exception)
		{
			aggregateException = new AggregateException(exception);
		}
		finally
		{
			IsCompleted = true;
			supplier = null;
			isResultReadyEvent.Set();

			lock (continuationQueueLocker)
			{
				while (continuations.Count != 0)
				{
					threadPool.Enqueue(continuations.Dequeue());
				}
			}
		}
	}

	/// <summary>
	/// Enqueues the task execution to the task's thread pool
	/// </summary>
	public void Run()
	{
		threadPool.Enqueue(StartTask);
	}

	/// <summary>
	/// Creates a new task which is applied to the result of another one
	/// </summary>
	/// <typeparam name="TNewResult">New task result type</typeparam>
	/// <param name="newSupplier">Function to be encapsulated into the new task</param>
	/// <returns>Created task</returns>
	public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newSupplier)
	{
		var task = new MyTask<TNewResult>(() => newSupplier(Result), threadPool);

		lock (continuationQueueLocker)
		{
			if (IsCompleted)
			{
				threadPool.Enqueue(task.StartTask);
			}
			else
			{
				continuations.Enqueue(task.StartTask);
			}
		}

		return task;
	}

	public MyTask(Func<TResult> supplier, IMyThreadPool threadPool)
	{
		this.threadPool = threadPool;
		this.supplier = supplier;
	}
}
