namespace ThreadPool
{
    public class MyThreadPool : IDisposable
    {
        private readonly List<MyThread> _threads;

        private readonly List<AutoResetEvent> _autoResetEvents;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly CancellationToken _cancellationToken;

        private readonly Random _random = new();

        private readonly object _locker = new();

        public MyThreadPool(int threadsSize, ThreadPoolWorkStrategy workStrategy = ThreadPoolWorkStrategy.Stealing)
        {
            if (threadsSize <= 0)
            {
                throw new ArgumentException("Threads size should be > 0");
            }
            ThreadPoolWorkStrategy = workStrategy;
            _threads = new List<MyThread>();
            _cancellationToken = _cancellationTokenSource.Token;
            _autoResetEvents = new List<AutoResetEvent>();

            for (var i = 0; i < threadsSize; ++i)
            {
                var thread = new MyThread(this, _cancellationToken, i);
                _threads.Add(thread);
                thread.Start();

                var autoResetEvent = new AutoResetEvent(false);
                _autoResetEvents.Add(autoResetEvent);
            }
        }

        public void Enqueue(Action task)
        {
            if (!_cancellationToken.IsCancellationRequested)
            {
                var threadWithoutTasks = _threads.Find(x => x.IsEmptyTasks);
                if (threadWithoutTasks != null)
                {
                    lock (_locker)
                    {
                        threadWithoutTasks.PushTask(task);
                        _autoResetEvents[threadWithoutTasks.Id].Set();
                    }
                }
                else
                {
                    int randomNumber = _random.Next() % _threads.Count;
                    lock (_locker)
                    {
                        _threads[randomNumber].PushTask(task);
                        _autoResetEvents[randomNumber].Set();
                    }
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            foreach (var thread in _threads)
            {
                thread.Join();
            }

            foreach (var autoResetEvent in _autoResetEvents)
            {
                autoResetEvent.Dispose();
            }

            _cancellationTokenSource.Dispose();
        }

        public int ThreadsSize => _threads.Count;

        public ThreadPoolWorkStrategy ThreadPoolWorkStrategy { get; }

        public List<MyThread> Threads { get { return _threads; } }

        public List<AutoResetEvent> AutoResetEvents { get { return _autoResetEvents; } }
    }
}
