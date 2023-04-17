using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Consumer
{
    public class Consumer
    {
        public async Task Consume(List<string> buffer, Connector.Connector connector, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string item = null;
                lock (buffer)
                {
                    if (buffer.Count > 0)
                    {
                        item = buffer[0];
                        buffer.RemoveAt(0);
                    }
                }
                if (item != null)
                {
                    connector.DecrementBufferCount();
                    Console.WriteLine($"Consumed: {item}");
                }
                await Task.Delay(TimeSpan.FromMilliseconds(3000), cancellationToken);
            }
        }
    }
}
