namespace Task2;

public class ProducerConsumerTask<T>
{
	private readonly Semaphore emptyQueueLock = new(0, 1);
	private readonly Semaphore queueAccessLock = new(1, 1);

	private readonly List<T> queue = new();

	public Task AddProducer(IProducer<T> producer, CancellationToken cancellationToken)
	{
		void Produce()
		{
			while (!cancellationToken.IsCancellationRequested && producer.CanProduce)
			{
				var produced = producer.Produce();

				queueAccessLock.WaitOne();
				var wasEmpty = queue.Count == 0;
				queue.Add(produced);

				if (wasEmpty)
				{
					emptyQueueLock.Release();
				}

				queueAccessLock.Release();
			}
		}

		return new Task(Produce);
	}

	public Task AddConsumer(IConsumer<T> consumer, CancellationToken cancellationToken)
	{
		void Consume()
		{
			var handles = new[] { cancellationToken.WaitHandle, emptyQueueLock };

			while (!cancellationToken.IsCancellationRequested)
			{
				WaitHandle.WaitAny(handles);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				var consumed = default(T);
				var wasConsumed = false;

				queueAccessLock.WaitOne();

				if (queue.Count > 0)
				{
					consumed = queue.Last();
					queue.Remove(consumed);
					wasConsumed = true;
				}

				if (queue.Count > 0)
				{
					emptyQueueLock.Release();
				}

				queueAccessLock.Release();

				if (wasConsumed)
				{
					consumer.Consume(consumed);
				}
			}
		}

		return new Task(Consume);
	}
}
