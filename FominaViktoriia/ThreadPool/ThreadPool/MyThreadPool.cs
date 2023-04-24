namespace ThreadPool
{
    public class MyThreadPool : IDisposable
    {
        private readonly List<MyThread> _threads;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly CancellationToken _cancellationToken;

        private readonly Random _random = new();

        public MyThreadPool(int threadsSize, ThreadPoolWorkStrategy workStrategy = ThreadPoolWorkStrategy.Stealing)
        {
            if (threadsSize <= 0)
            {
                throw new ArgumentException("Threads size should be > 0");
            }
            ThreadPoolWorkStrategy = workStrategy;
            _threads = new List<MyThread>();
            _cancellationToken = _cancellationTokenSource.Token;

            for (var i = 0; i < threadsSize; ++i)
            {
                var thread = new MyThread(this, _cancellationToken);
                _threads.Add(thread);
                thread.Start();
            }
        }

        public void Enqueue(Action task)
        {
            if (!_cancellationToken.IsCancellationRequested)
            {
                var threadWithoutTasks = _threads.Find(x => x.IsEmptyTasks);
                if (threadWithoutTasks != null)
                {
                    threadWithoutTasks.PushTask(task);
                }
                else
                {
                    _threads[_random.Next() % _threads.Count].PushTask(task);
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public int ThreadsSize => _threads.Count;

        public ThreadPoolWorkStrategy ThreadPoolWorkStrategy { get; }

        public List<MyThread> Threads { get { return _threads; } }
    }
}
