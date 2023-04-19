using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.ILocks
{
    public interface ILock
    {
        public void Lock();
        public void Unlock();
    }
}
