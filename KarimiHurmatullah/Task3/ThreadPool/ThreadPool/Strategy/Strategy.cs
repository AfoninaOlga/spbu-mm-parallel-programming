using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadPool.ThreadPool;

namespace ThreadPool.Strategy
{
    internal class Strategy
    {
        private readonly ConcurrentQueue<Action> waitingTasksQueue = new ConcurrentQueue<Action>();

        private readonly CancellationToken cancellationToken = new CancellationToken();

        internal int TasksCount => waitingTasksQueue.Count + (isTaskWaiting ? 0 : 1);

        private readonly ThreadPool.ThreadPool myThreadPoolInstance;

        private readonly WorkStrategy workStrategy;

        private readonly Thread threadInstance;

        private bool isTaskWaiting = true;

        private const int Threshold = 4;

        internal int Id { get; }

        internal Strategy(ThreadPool.ThreadPool threadPool, int id, CancellationToken cancelToken)
        {
            myThreadPoolInstance = threadPool;
            Id = id;
            cancellationToken = cancelToken;
            threadInstance = new Thread(Process);
        }

        internal void AddTaskToQueue(Action task)
        {
            waitingTasksQueue.Enqueue(task);
        }

        internal void StartThreads()
        {
            threadInstance.Start();
        }

        private void Process()
        {
            while (true)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (waitingTasksQueue.TryDequeue(out var task))
                    {
                        isTaskWaiting = false;
                        task?.Invoke();
                        ExecuteTaskAndShareTask();
                    }
                    else if (workStrategy == WorkStrategy.WorkStealing && myThreadPoolInstance.StealTasks(out task, Id))
                    {
                        isTaskWaiting = false;
                        task?.Invoke();
                        ExecuteTaskAndShareTask();
                    }
                    else
                    {
                        Thread.Yield();
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            foreach (var task in waitingTasksQueue)
            {
                task.Invoke();
            }
        }

        internal void ExecuteTaskAndShareTask()
        {
            if (workStrategy == WorkStrategy.WorkSharing)
            {
                myThreadPoolInstance.ShareTasks(Id);
            }
            isTaskWaiting = true;
        }

        internal bool StealTasks(out Action? task) => waitingTasksQueue.TryDequeue(out task);

        internal void BalanceQueue(Strategy mark)
        {
            Strategy minimum, maximum;

            if (this.waitingTasksQueue.Count < mark.waitingTasksQueue.Count)
            {
                minimum = this;
                maximum = mark;
            }
            else
            {
                minimum = mark;
                maximum = this;
            }

            var diff = maximum.waitingTasksQueue.Count - minimum.waitingTasksQueue.Count;

            if (diff <= Threshold)
            {
                return;
            }

            while (maximum.waitingTasksQueue.Count > minimum.waitingTasksQueue.Count)
            {
                maximum.waitingTasksQueue.TryDequeue(out var task);

                if (task != null)
                {
                    minimum.waitingTasksQueue.Enqueue(task);
                }
            }
        }

        internal void Join()
        {
            threadInstance.Join();
        }
    }
}
