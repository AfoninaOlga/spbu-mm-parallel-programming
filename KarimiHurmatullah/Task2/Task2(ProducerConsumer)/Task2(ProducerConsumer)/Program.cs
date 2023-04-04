using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2_ProducerConsumer_
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var pc = new ProducerConsumer();
            await pc.Run();
        }

    }
}
