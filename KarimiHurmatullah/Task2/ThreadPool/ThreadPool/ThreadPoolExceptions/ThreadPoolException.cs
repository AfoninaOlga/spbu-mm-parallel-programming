using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadPool.ThreadPoolExceptions
{
    public class ThreadPoolException : Exception
    {
        private readonly string message;

        public ThreadPoolException(string message) : base(message)
        {
            this.message = message;
        }
    }
}
