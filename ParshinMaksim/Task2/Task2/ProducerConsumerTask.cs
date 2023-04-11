namespace Task2;

public interface IProducer
{
    public Task Start(CancellationToken cancellationToken);
}

public interface IConsumer
{
    public Task Start(CancellationToken cancellationToken);
}

public class ProducerConsumerTask<T>
{
    private readonly Semaphore _emptyQueueLock = new(0, 1);
    private readonly Semaphore _queueAccessLock = new(1, 1);

    private readonly List<T> _queue = new();

    private class Producer : IProducer

    {
        private readonly Func<T> _produce;
        private readonly Func<bool> _canProduce;
        private readonly ProducerConsumerTask<T> _task;

        public Producer(Func<T> produce, Func<bool> canProduce, ProducerConsumerTask<T> task)
        {
            _produce = produce;
            _canProduce = canProduce;
            _task = task;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            void ProducerAction()
            {
                while (!cancellationToken.IsCancellationRequested && _canProduce())
                {
                    var produced = _produce();

                    _task._queueAccessLock.WaitOne();
                    var wasEmpty = _task._queue.Count == 0;
                    _task._queue.Add(produced);

                    if (wasEmpty)
                    {
                        _task._emptyQueueLock.Release();
                    }

                    _task._queueAccessLock.Release();
                }
            }

            return Task.Run(ProducerAction);
        }
    }

    private class Consumer : IConsumer
    {
        private readonly Action<T> _consume;
        private readonly ProducerConsumerTask<T> _task;

        public Consumer(Action<T> consume, ProducerConsumerTask<T> task)
        {
            _consume = consume;
            _task = task;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            void ConsumerAction()
            {
                var handles = new[] { cancellationToken.WaitHandle, _task._emptyQueueLock };

                while (!cancellationToken.IsCancellationRequested)
                {
                    WaitHandle.WaitAny(handles);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    var consumed = default(T);
                    var wasConsumed = false;

                    _task._queueAccessLock.WaitOne();

                    if (_task._queue.Count > 0)
                    {
                        consumed = _task._queue.Last();
                        _task._queue.Remove(consumed);
                        wasConsumed = true;
                    }

                    if (_task._queue.Count > 0)
                    {
                        _task._emptyQueueLock.Release();
                    }

                    _task._queueAccessLock.Release();

                    if (wasConsumed)
                    {
                        _consume(consumed);
                    }
                }
            }

            return Task.Run(ConsumerAction);
        }
    }

    public IProducer AddProducer(Func<T> produce, Func<bool>? canProduce = null)
    {
        if (canProduce is not null)
        {
            return new Producer(produce, canProduce, this);
        }

        return new Producer(produce, () => true, this);
    }

    public IConsumer AddConsumer(Action<T> consume) =>
        new Consumer(consume, this);
}
