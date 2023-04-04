using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Task2_ThreadPool_
{
    public class ThreadPool : IDisposable
    {
        private readonly int _maximumThreadPoolCount;
        private readonly Thread[] _threads;
        private readonly ConcurrentQueue<Action> _queueOfTasks = new ConcurrentQueue<Action>();
        private readonly AutoResetEvent _event = new AutoResetEvent(true);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public bool IsTerminated
        {
            get;
            private set;
        }

        public ThreadPool(int maximumThreadsCount)
        {
            if (maximumThreadsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumThreadsCount),
                "The number of threads in the pool must be greater than zero.");
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
                _threads[threadIndex] = new Thread(() => {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        if (!_queueOfTasks.IsEmpty)
                        {
                            _queueOfTasks.TryDequeue(out
                              var result);
                            result?.Invoke();
                        }
                    }
                });

                _threads[threadIndex].Start();
            }
        }
        public MyTask<TResult> Submit<TResult>(Func<TResult> function)
        {
            _event.WaitOne();
            var task = new MyTask<TResult>(function, this);
            _queueOfTasks.Enqueue(() => task.Start());
            _event.Set();
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
                _event.Dispose();
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
            if(disposing)
            {
                Shutdown();
            }

            _event.Dispose();
            _cancellationTokenSource.Dispose();
        }

    }
}
