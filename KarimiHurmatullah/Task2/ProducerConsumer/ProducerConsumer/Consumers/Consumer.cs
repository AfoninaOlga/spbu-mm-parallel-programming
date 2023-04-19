using ProducerConsumer.DataVerification;
using ProducerConsumer.ILocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Consumers
{
    public class Consumer
    {
        private List<DataContainer<int>> tasks;

        private volatile bool stopped;

        private Thread thread;

        private ILock locker;

        public Consumer(ILock locker, List<DataContainer<int>> tasks)
        {
            this.locker = locker;
            thread = new Thread(Consume);
            this.tasks = tasks;
            Run();
        }

        private void Consume()
        {
            while (!stopped)
            {
                locker.Lock();

                if (tasks.Count > 0)
                {
                    var data = tasks[0];
                    Console.WriteLine($"Consumer {Thread.CurrentThread.ManagedThreadId} consumed {data.Inf}");
                    tasks.RemoveAt(0);
                }

                locker.Unlock();    
                Thread.Sleep(500);
            }
        }

        public void Run()
        {
            stopped = false;
            thread.Start();
        }

        private void Stop()
        {
            stopped = true;
        }

        public void Join()
        {
            Stop();
            thread.Join();
        }
    }
}
