using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadPool.IMyTasks;

namespace ThreadPool.ThreadPools
{
    public class ThreadPool : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly ConcurrentQueue<Action> _queueOfTasks = new ConcurrentQueue<Action>();

        private AutoResetEvent _taskAvailableEvent = new AutoResetEvent(false);

        private readonly int _maximumThreadPoolCount;

        private readonly Thread[] _threads;

        private int _nextThreadIndex = 0;

        public bool IsTerminated
        {
            get;
            private set;
        }

        public ThreadPool(int maximumThreadsCount)
        {
            if (maximumThreadsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumThreadsCount), "The number of threads in the pool must be greater than zero.");
            }
            _maximumThreadPoolCount = maximumThreadsCount;
            IsTerminated = false;
            _threads = new Thread[maximumThreadsCount];
            StartThreads();
        }

        private void StartThreads()
        {
            for (var threadIndex = 0; threadIndex < _maximumThreadPoolCount; ++threadIndex)
            {
                _threads[threadIndex] = new Thread(() =>
                {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        if (_queueOfTasks.TryDequeue(out var result))
                        {
                            result?.Invoke();
                        }
                        else
                        {
                            StealTasks();
                            _taskAvailableEvent.WaitOne(0);
                        }
                    }
                });
                _threads[threadIndex].Start();
            }
        }

        private void StealTasks()
        {
            for (var i = 0; i < _maximumThreadPoolCount; i++)
            {
                var randomIndex = new Random().Next(_maximumThreadPoolCount);
                if (randomIndex != _nextThreadIndex && _queueOfTasks.TryDequeue(out var stolenTask))
                {
                    _queueOfTasks.Enqueue(stolenTask);
                    break;
                }
            }
            Interlocked.Increment(ref _nextThreadIndex);
            _nextThreadIndex %= _maximumThreadPoolCount;
        }

        public MyTask<TResult> Submit<TResult>(Func<TResult> function)
        {
            var currentThreadIndex = _nextThreadIndex;
            _nextThreadIndex = (_nextThreadIndex + 1) % _maximumThreadPoolCount;

            var task = new MyTask<TResult>(function, this);
            _queueOfTasks.Enqueue(() =>
            {
                task.Start();
                _taskAvailableEvent.Set();
            });
            return task;
        }

        public void Shutdown()
        {
            if (!IsTerminated)
            {
                _cancellationTokenSource.Cancel();
                for (var i = 0; i < _maximumThreadPoolCount; ++i)
                {
                    _threads[i].Join();
                }
                _cancellationTokenSource.Dispose();
            }
            IsTerminated = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Shutdown();
            }
            _cancellationTokenSource.Dispose();
        }

    }
}
