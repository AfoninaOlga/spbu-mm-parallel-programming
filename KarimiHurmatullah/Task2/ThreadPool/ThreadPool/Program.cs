using ThreadPool.IMyTasks;

namespace ThreadPool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var threadPool = new ThreadPools.ThreadPool(12);
            var tasksList = new List<MyTask<int>>();

            for (var threadIndex = 10; threadIndex >= 1; --threadIndex)
            {
                var tid = threadIndex;
                var function = new Func<int>(() => {
                    Console.WriteLine($"task number {tid} accepted for executation");
                    return (int)Math.Pow(2, tid);
                });

                var task = new MyTask<int>(function, threadPool);
                tasksList.Add(task);
            }

            foreach (var task in tasksList)
            {
                var result = task.Result;

                Console.WriteLine($"Result = {result}");
                var newTask = task.ContinueWith(radius => {
                    var newValue = (int)Math.Log(radius) / 2;
                    Console.WriteLine($"Task number {newValue} accepted for execution");
                    return (int)Math.Pow(2, newValue);
                });
                var newResult = newTask.Result;
                Console.WriteLine($"Continuation {(int)Math.Log(result) / 2} Result = {newResult}");
            }
            threadPool.Shutdown();
        }
    }
}