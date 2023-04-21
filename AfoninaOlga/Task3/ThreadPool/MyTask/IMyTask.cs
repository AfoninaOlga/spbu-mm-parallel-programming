namespace ThreadPool.MyTask;

public interface IMyTask<out TResult> : IDisposable
{
    bool IsCompleted { get; }

    TResult Result { get; }

    void Run();

    IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
}