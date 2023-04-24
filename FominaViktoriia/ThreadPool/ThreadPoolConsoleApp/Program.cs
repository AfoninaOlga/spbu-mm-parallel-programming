using System.Threading;

namespace ThreadPool
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            using (MyThreadPool threadPool = new(4))
            {
                var tasksSize = 15;
                var tasks = new MyTask<int>[tasksSize];
                for (var i = 0; i < tasksSize; ++i)
                {
                    tasks[i] = new MyTask<int>(() => { Thread.Sleep(100); return 2 + 2; }, threadPool);
                }

                for (var i = 0; i < tasksSize; ++i)
                {
                    tasks[i].Start();
                }
            }
        }
    }
}