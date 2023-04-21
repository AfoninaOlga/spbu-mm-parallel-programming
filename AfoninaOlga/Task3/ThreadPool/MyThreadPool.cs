using System.Collections.Concurrent;
using ThreadPool.MyTask;

namespace ThreadPool;

public class MyThreadPool : IDisposable
{
    private const int DefaultSize = 4;

    private readonly Worker[] _workers;
    private readonly CancellationTokenSource _cts = new();
    private readonly WorkStrategy _strategy;
    private readonly object _disposeLockObj = new();

    private readonly Random _random = new();

    private bool _isDisposed;

    public int Size => _workers.Count(worker => worker.IsAlive);

    public MyThreadPool(int poolSize = DefaultSize, WorkStrategy strategy = WorkStrategy.Stealing)
    {
        _strategy = strategy;
        _workers = new Worker[poolSize];
        for (var i = 0; i < poolSize; ++i)
        {
            _workers[i] = new Worker(this, i, _cts.Token);
        }

        foreach (var worker in _workers)
        {
            worker.Start();
        }
    }
    
    public void Enqueue<TResult>(IMyTask<TResult> task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task), "Cannot run null in ThreadPool");
        }

        lock (_disposeLockObj)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Cannot add task into ThreadPool because it's disposed");
            }

            var workerId = _workers
                .OrderBy(worker => worker.TasksCount)
                .First()
                .Id;
            _workers[workerId].AddTask(task.Run);
        }
    }

    public void Dispose()
    {
        lock (_disposeLockObj)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Cannot dispose ThreadPool because it's already disposed");
            }

            _isDisposed = true;
            _cts.Cancel();
            foreach (var worker in _workers)
            {
                worker.Join();
            }

            _cts.Dispose();
        }
    }

    private bool TryStealTask(out Action? task, int id)
    {
        var victimId = (id + _random.Next(_workers.Length - 1)) % _workers.Length;
        return _workers[victimId].TryStealTask(out task);
    }

    private void ShareTasks(int id)
    {
        var size = _workers[id].TasksCount;
        if (_random.Next(size + 1) == size)
        {
            var victimId = (id + _random.Next(_workers.Length - 1)) % _workers.Length;
            var min = (victimId <= id) ? victimId : id;
            var max = (victimId <= id) ? id : victimId;
            lock (_workers[min])
            {
                lock (_workers[max])
                {
                    _workers[min].Balance(_workers[max]);
                }
            }
        }
    }

    private sealed class Worker
    {
        private const int Threshold = 1;
        
        private readonly MyThreadPool _myThreadPool;
        private readonly Thread _thread;
        private readonly CancellationToken _ct;
        private readonly ConcurrentQueue<Action> _waitingTasks = new();
        private bool _isWaiting = true;

        internal int Id { get; }
        internal bool IsAlive => _thread.IsAlive;
        internal int TasksCount => _waitingTasks.Count + (_isWaiting ? 0 : 1);
    
        internal Worker(MyThreadPool myThreadPool, int id, CancellationToken ct)
        {
            _myThreadPool = myThreadPool;
            Id = id;
            _ct = ct;
            _thread = new Thread(Work);
        }
    
        internal void AddTask(Action task)
        {
            _waitingTasks.Enqueue(task);
        }
    
        internal void Start()
        {
            _thread.Start();
        }
    
        private void Work()
        {
            while (true)
            {
                try
                {
                    _ct.ThrowIfCancellationRequested();
                    if (_waitingTasks.TryDequeue(out var task) || _myThreadPool._strategy == WorkStrategy.Stealing
                                                                            && _myThreadPool.TryStealTask(out task, Id))
                    {
                        _isWaiting = false;
                        task?.Invoke();
                        if (_myThreadPool._strategy == WorkStrategy.Sharing)
                        {
                            _myThreadPool.ShareTasks(Id);
                        }
                        _isWaiting = true;
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
    
            foreach (var task in _waitingTasks)
            {
                task.Invoke();
            }
        }
    
        internal bool TryStealTask(out Action? task) => _waitingTasks.TryDequeue(out task);
    
        internal void Balance(Worker victim)
        {
            var qMin = (this._waitingTasks.Count < victim._waitingTasks.Count) ? this : victim;
            var qMax = (this._waitingTasks.Count < victim._waitingTasks.Count) ? victim : this;
            var diff = qMax._waitingTasks.Count - qMin._waitingTasks.Count;
            if (diff <= Threshold) return;
            while (qMax._waitingTasks.Count > qMin._waitingTasks.Count)
            {
                qMax._waitingTasks.TryDequeue(out var task);
                if (task != null) qMin._waitingTasks.Enqueue(task);
            }
        }
    
        internal void Join()
        {
            _thread.Join();
        }
    }
}