using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumer.Connector
{
    internal class Connector
    {
        private readonly List<string> Buffer = new List<string>();
        private const int NumProducers = 2;
        private const int NumConsumers = 2;
        private int ActiveProducers;
        private int ActiveConsumers;
        private readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public async Task Run()
        {
            var tasks = new List<Task>();

            var producer = new Producer.Producer();
            for (int i = 0; i < NumProducers; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    if (Interlocked.Increment(ref ActiveProducers) <= NumProducers)
                    {
                        await producer.Produce(Buffer, CancellationTokenSource.Token);
                        Interlocked.Decrement(ref ActiveProducers);
                    }
                }));
            }

            var consumer = new Consumer.Consumer();
            for (int i = 0; i < NumConsumers; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    if (Interlocked.Increment(ref ActiveConsumers) <= NumConsumers)
                    {
                        await consumer.Consume(Buffer, CancellationTokenSource.Token);
                        Interlocked.Decrement(ref ActiveConsumers);
                    }
                }));
            }

            Console.ReadKey();
            CancellationTokenSource.Cancel();
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("\n Producers and Consumers processes are shutt down");
            }
        }

    }
}
