using System.Collections.Concurrent;

namespace ThreadPool
{
    public class MyThread
    {
        private Thread? _thread;

        private readonly List<MyThread> _threadsInThreadPool;

        private readonly ConcurrentQueue<Action> _tasks = new();

        private readonly CancellationToken _cancellationToken;

        private readonly ThreadPoolWorkStrategy _workStrategy;

        private readonly int _waitTimeMSec = 100;

        private readonly Random _random = new();

        public MyThread(MyThreadPool threadPool, CancellationToken cancellationToken)
        {
            this._threadsInThreadPool = threadPool.Threads;
            this._cancellationToken = cancellationToken;
            this._workStrategy = threadPool.ThreadPoolWorkStrategy;
        }

        public void Start()
        {
            _thread = new Thread(() =>
            {
                try
                {
                    if (this._workStrategy == ThreadPoolWorkStrategy.Stealing)
                    {
                        while (true)
                        {
                            if (IsEmptyTasks && !_cancellationToken.IsCancellationRequested)
                            {
                                var threadWithTasks = _threadsInThreadPool.FirstOrDefault(t => !t.IsEmptyTasks);
                                if (threadWithTasks != null)
                                {
                                    var task = threadWithTasks.PopTask();
                                    task?.Invoke();
                                }
                                else
                                {
                                    Thread.Sleep(_waitTimeMSec);
                                }
                            }
                            else if (!IsEmptyTasks)
                            {
                                var myTask = PopTask();
                                myTask?.Invoke();
                            }
                            else
                            {
                                Thread.Sleep(_waitTimeMSec);
                            }

                            if (_cancellationToken.IsCancellationRequested)
                            {
                                _thread?.Interrupt();
                            }
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            // Thus, we can share tasks.
                            if (_tasks.Count > 1 && !_cancellationToken.IsCancellationRequested)
                            {
                                var taskToShare = PopTask();
                                if (taskToShare != null)
                                {
                                    _threadsInThreadPool[_random.Next() % _threadsInThreadPool.Count].PushTask(taskToShare);
                                }
                            }

                            if (!IsEmptyTasks)
                            {
                                var task = PopTask();
                                task?.Invoke();
                            }
                            else
                            {
                                Thread.Sleep(_waitTimeMSec);
                            }

                            if (_cancellationToken.IsCancellationRequested)
                            {
                                _thread?.Interrupt();
                            }
                        }
                    }
                }
                catch (ThreadInterruptedException) { }
            });
            _thread.Start();
        }

        internal bool IsEmptyTasks => _tasks.IsEmpty;

        internal void PushTask(Action task)
        {
            _tasks.Enqueue(task);
        }

        internal Action? PopTask()
        {
            var dequeued = _tasks.TryDequeue(out var task);
            if (!dequeued)
            {
                return null;
            }

            return task;
        }
    }
}
