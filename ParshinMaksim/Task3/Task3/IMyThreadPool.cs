using System;

namespace Task3;

/// <summary>
/// Interface representing thread pool
/// </summary>
public interface IMyThreadPool : IDisposable
{
	/// <summary>
	/// Cancellation token of the pool
	/// </summary>
    CancellationToken CancellationToken { get; }

    /// <summary>
    /// Creates a new task which result is evaluated by the thread pool
    /// </summary>
    /// <typeparam name="TResult">Task result type</typeparam>
    /// <param name="supplier">Function to be encapsulated into the new task</param>
    /// <returns>Created task</returns>
    IMyTask<TResult> Enqueue<TResult>(Func<TResult> supplier);

    /// <summary>
    /// Enqueues an action to the thread pool
    /// </summary>
    /// <param name="supplier">Action to enqueue</param>
    void Enqueue(Action supplier);

    /// <summary>
    /// Stops the thread pool work
    /// </summary>
    void Shutdown();
}
