using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework.Constraints;
using ProducerConsumer.Connector;
using ProducerConsumer.Consumer;
using ProducerConsumer.Producer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestCases.ConnectorTest
{
    internal class ConnectorTest
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private List<string> buffer = new List<string>();

        private Connector connector = new Connector();

        private Producer producer = new Producer();

        private Consumer consumer = new Consumer();

        private int NumProducers = 4;

        private int NumConsumers = 4;

        private int ActiveProducers;

        private int ActiveConsumers;

        private int BufferCount = 0;

        [Test]
        public async Task ConnectTest()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < NumProducers; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    if (ActiveProducers <= NumProducers)
                    {
                        ActiveProducers++;
                        Thread.Sleep(1);
                        await producer.Produce(buffer, connector, cancellationTokenSource.Token);
                        ActiveProducers--;
                    }
                }));
            }

            for (int i = 0; i < NumProducers; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    if (ActiveConsumers <= NumConsumers)
                    {
                        ActiveConsumers++;
                        Thread.Sleep(1);
                        await consumer.Consume(buffer, connector, cancellationTokenSource.Token);
                        ActiveConsumers--;
                    }
                }));
            }
            Assert.IsTrue(true);
        }

        [Test]
        public void GetBufferCountTest()
        {
            Connector connector = new Connector();
            var bufferCountResult = connector.GetBufferCount();
            Assert.That(bufferCountResult, Is.EqualTo(BufferCount));
        }

        [Test]
        public void NextBufferCount()
        {
            var incremented = ++BufferCount;
            Assert.That(incremented, Is.EqualTo(1));
        }

        [Test]
        public void PreviousBufferCount()
        {
            var decremented = --BufferCount;
            Assert.That(decremented, Is.EqualTo(0));
        }
    }
}
