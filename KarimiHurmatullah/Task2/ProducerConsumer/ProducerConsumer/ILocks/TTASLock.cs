using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.ILocks
{
    public class TTASLock : ILock
    {
        private volatile int state = 0;

        public void Lock()
        {
            while (true)
            {
                while (1 == state)
                {
                    Thread.Sleep(0);
                }

                if (0 == Interlocked.CompareExchange(ref state, 1, 0))
                {
                    return;
                }
            }
        }

        public void Unlock()
        {
            state = 0;
        }
    }
}
