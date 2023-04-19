using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.DataVerification
{
    public class DataContainer<T>
    {
        public T Inf { get; set; }

        public DataContainer(T data)
        {
            Inf = data;
        }
    }
}
