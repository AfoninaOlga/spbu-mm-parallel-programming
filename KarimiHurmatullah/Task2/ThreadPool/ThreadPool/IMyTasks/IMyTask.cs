using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPool.IMyTasks
{
    public interface IMyTask<TResult>
    {
        bool IsCompleted
        {
            get;
        }
        TResult Result
        {
            get;
        }
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function);
    }

    public class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly Func<TResult> _function;
        private readonly ThreadPools.ThreadPool _threadPool;
        private readonly AutoResetEvent _event = new AutoResetEvent(true);
        private TResult _result;

        public bool IsCompleted
        {
            get;
            private set;
        }

        public TResult Result
        {
            get
            {
                _event.WaitOne();

                if (_hasException) { throw _taskException; }

                if (!IsCompleted) { Start(); }

                _event.Set();
                return _result;
            }
        }

        private AggregateException _taskException;
        private bool _hasException = false;

        public MyTask(Func<TResult> function, ThreadPools.ThreadPool threadPool)
        {
            _function = function;
            _threadPool = threadPool;
            IsCompleted = false;
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> function)
        {
            if (_threadPool.IsTerminated)
            {
                _taskException = new AggregateException("Unable to execute task because the thread pool has been terminated",
                  new ThreadPoolExceptions.ThreadPoolException("The thread pool encountered an error while executing a task. See inner exception for details."));
                _hasException = true;
            }

            return _threadPool.Submit(() => function(Result));
        }

        public void Start()
        {
            try
            {
                _result = _function();
                IsCompleted = true;
            }
            catch (Exception e)
            {
                _hasException = true;
                _taskException = new AggregateException("The task has not completed, so a result is not available", e);
            }
        }
    }
}
