namespace ThreadPool
{
    public interface IMyTask<TResult>
    {
        public bool IsCompleted { get; }

        public TResult Result { get; }

        public MyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> apply);
    }
}
