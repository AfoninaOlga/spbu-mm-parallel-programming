# ThreadPool problem

Implementation of thread pool with work-stealing and work-sharing strategies.

## Dependencies

- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)

## Build project

```sh
dotnet build
```

## Run tests

```sh
dotnet test
```

## Example
Usage example of `MyThreadPool` with 4 threads and work-stealing strategy. Add  ing tasks with continuation.
```csharp
using ThreadPool;
using ThreadPool.MyTask;

using var threadPool = new MyThreadPool(4, WorkStrategy.Stealing);
using var task = new MyTask<string>(() => "Task finished");
threadPool.Enqueue(task);    
using var continuation = task.ContinueWith(a => a + " and its continuation finished");
threadPool.Enqueue(continuation);

Console.WriteLine(continuation.Result);
```