using ProducerConsumer.Consumers;
using ProducerConsumer.DataVerification;
using ProducerConsumer.ILocks;
using ProducerConsumer.Producers;

namespace ProducerConsumer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Invalid command-line arguments. Expected at least two arguments, but received " + args.Length + ".");
            }

            var producerCount = ConsoleInputVerifier.ReadInputFromConsole(args[0]);
            var consumerCount = ConsoleInputVerifier.ReadInputFromConsole(args[1]);

            if (producerCount == 0 || consumerCount == 0)
            {
                throw new ArgumentException("Invalid argument(s): Both arguments must be positive integers.");
            }

            var tasks = new List<DataContainer<int>>();
            var locker = new TTASLock();

            var producers = Enumerable.Range(0, consumerCount)
                                      .Select(i => new Producer(locker, tasks))
                                      .ToArray();

            var consumers = Enumerable.Range(0, producerCount)
                                      .Select(i => new Consumer(locker, tasks))
                                      .ToArray();

            Console.ReadKey();

            producers.ToList().ForEach(worker => worker.Join());
            consumers.ToList().ForEach(worker => worker.Join());
            tasks.ForEach(worker => Console.WriteLine(worker.Inf));
        }
    }
}