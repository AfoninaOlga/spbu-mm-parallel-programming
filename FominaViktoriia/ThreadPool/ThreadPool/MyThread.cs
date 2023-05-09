using System.Collections.Concurrent;

namespace ThreadPool
{
    public class MyThread
    {
        private Thread? _thread;

        private int _id;

        private readonly List<MyThread> _threadsInThreadPool;

        private readonly List<AutoResetEvent> _autoResetEventsInThreadPool;

        private readonly ConcurrentQueue<Action> _tasks = new();

        private readonly CancellationToken _cancellationToken;

        private readonly ThreadPoolWorkStrategy _workStrategy;

        private readonly int _waitTimeMSec = 100;

        private readonly Random _random = new();

        private readonly object _locker = new();

        public MyThread(MyThreadPool threadPool, CancellationToken cancellationToken, int threadId)
        {
            this._threadsInThreadPool = threadPool.Threads;
            this._autoResetEventsInThreadPool = threadPool.AutoResetEvents;
            this._cancellationToken = cancellationToken;
            this._workStrategy = threadPool.ThreadPoolWorkStrategy;
            this._id = threadId;
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
                                    _autoResetEventsInThreadPool[_id].WaitOne(_waitTimeMSec);                                                                        
                                }
                            }
                            else if (!IsEmptyTasks)
                            {
                                var myTask = PopTask();
                                myTask?.Invoke();
                            }
                            else
                            {
                                _autoResetEventsInThreadPool[_id].WaitOne(_waitTimeMSec);
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
                                    int randomNumber = _random.Next() % _threadsInThreadPool.Count;
                                    lock (_locker)
                                    {
                                        _threadsInThreadPool[randomNumber].PushTask(taskToShare);
                                        _autoResetEventsInThreadPool[randomNumber].Set();
                                    }
                                }
                            }

                            if (!IsEmptyTasks)
                            {
                                var task = PopTask();
                                task?.Invoke();
                            }
                            else
                            {
                                _autoResetEventsInThreadPool[_id].WaitOne(_waitTimeMSec);
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

        public void Join()
        {
            if (_thread == null)
            {
                throw new InvalidOperationException("Thread was not started");
            }

            _thread!.Join();
        }

        public int Id { get { return _id; } }

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
