using ProducerConsumer.DataVerification;
using ProducerConsumer.ILocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Producers
{
    public class Producer
    {
        private List<DataContainer<int>> tasks;

        private volatile bool stopped;

        private Thread thread;

        private ILock locker;

        public Producer(ILock locker, List<DataContainer<int>> tasks)
        {
            this.locker = locker;
            thread = new Thread(Produce);
            this.tasks = tasks;
            Run();
        }

        private void Produce()
        {
            while (!stopped)
            {
                locker.Lock();
                tasks.Add(new DataContainer<int>(Thread.CurrentThread.ManagedThreadId));
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
