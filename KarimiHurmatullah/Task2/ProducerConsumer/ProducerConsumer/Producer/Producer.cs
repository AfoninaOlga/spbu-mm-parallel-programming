using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Producer
{
    internal class Producer
    {
        public async Task Produce(List<string> buffer, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string item = DateTime.Now.ToString("HH:mm:ss.fff");

                lock (buffer)
                {
                    buffer.Add(item);
                }

                Console.WriteLine($"Produced: {item}");
                await Task.Delay(TimeSpan.FromMilliseconds(4000), cancellationToken);
            }
        }
    }
}
