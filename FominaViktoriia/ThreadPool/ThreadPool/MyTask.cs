using System.Collections.Concurrent;

namespace ThreadPool
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly Func<TResult> _func;

        private readonly MyThreadPool _threadPool;

        private TResult? _result;

        private ConcurrentQueue<Action> _continueWith = new();

        private AggregateException? _aggregateException;

        private readonly int _waitTimeMSec = 100;

        private object _completedLocker = new();

        private volatile bool _isCompleted = false;

        public MyTask(Func<TResult> func, MyThreadPool threadPool)
        {
            this._func = func;
            this._threadPool = threadPool;
        }

        public bool IsCompleted
        { 
            get { return _isCompleted; }
            private set { _isCompleted = value; }
        }

        public TResult Result
        {
            get
            {
                while (!IsCompleted)
                {
                    Thread.Sleep(_waitTimeMSec);
                }

                if (_aggregateException != null)
                {
                    throw _aggregateException;
                }

                return _result!;
            }

            private set => _result = value;
        }

        public void Start() => this._threadPool.Enqueue(this.RunTask);

        public MyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> apply)
        {
            if (_aggregateException != null)
            {
                throw _aggregateException;
            }

            if (IsCompleted)
            {
                throw new InvalidOperationException("Cannot call ContinueWith on calculated task");
            }

            var continueAction = () => apply(Result);
            var continueTask = new MyTask<TNewResult>(continueAction, _threadPool);

            lock (_completedLocker)
            {
                if (IsCompleted)
                {
                    _threadPool.Enqueue(continueTask.RunTask);
                }
                else
                {
                    _continueWith.Enqueue(continueTask.RunTask);
                }
            }

            return continueTask;
        }

        private void RunTask()
        {
            try
            {
                _result = _func();
            }
            catch (Exception e)
            {
                _aggregateException = new AggregateException(e);
            }
            finally
            {
                IsCompleted = true;
                while (!_continueWith.IsEmpty)
                {
                    var dequeued = _continueWith.TryDequeue(out var continueAction);
                    if (dequeued)
                    {
                        _threadPool.Enqueue(continueAction!);
                    }
                }
            }
        }
    }
}
