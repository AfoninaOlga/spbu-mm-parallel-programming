using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Task2_ProducerConsumer_
{
    class ProducerConsumer
    {
        private readonly LinkedList<string> Buffer = new LinkedList<string>();
        private const int NumProducers = 2;
        private const int NumConsumers = 2;
        private int ActiveProducers;
        private int ActiveConsumers;
        private readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public async Task Run()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < NumProducers; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    if (Interlocked.Increment(ref ActiveProducers) <= NumProducers)
                    {
                        await Producer(CancellationTokenSource.Token);
                        Interlocked.Decrement(ref ActiveProducers);
                    }
                }));
            }

            for (int i = 0; i < NumConsumers; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    if (Interlocked.Increment(ref ActiveConsumers) <= NumConsumers)
                    {
                        await Consumer(CancellationTokenSource.Token);
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
                Console.WriteLine("\n Producers and Consumers processes are shutting down");
            }
        }

        private async Task Producer(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string item = DateTime.Now.ToString("HH:mm:ss.fff");

                lock (Buffer)
                {
                    Buffer.AddLast(item);
                }

                Console.WriteLine($"Produced: {item}");
                await Task.Delay(TimeSpan.FromMilliseconds(6000), cancellationToken);
            }
        }

        private async Task Consumer(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                string item = null;

                lock (Buffer)
                {
                    if (Buffer.Count > 0)
                    {
                        item = Buffer.First.Value;
                        Buffer.RemoveFirst();
                    }
                }

                if (item != null)
                {
                    Console.WriteLine($"Consumed: {item}");
                }

                await Task.Delay(TimeSpan.FromMilliseconds(5000), cancellationToken);
            }
        }
    }

}
