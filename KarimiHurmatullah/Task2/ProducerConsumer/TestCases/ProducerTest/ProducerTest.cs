using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProducerConsumer.Producer;
using ProducerConsumer.Connector;
using NUnit.Framework;
using System.Threading;
using System.Globalization;

namespace TestCases.ProducerTest
{
    [TestFixture]
    internal class ProducerTest
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private List<string> buffer = new List<string>();

        private Connector connector = new Connector();

        private Producer producer = new Producer();

        [Test]
        public async Task ProduceTest()
        {
            var produceTask = producer.Produce(buffer, connector, cancellationTokenSource.Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));
            cancellationTokenSource.Cancel();
            await produceTask.ContinueWith(t => { });

            Assert.IsTrue(buffer.Count > 0);
            Assert.That(connector.GetBufferCount, Is.EqualTo(buffer.Count));
        }
    }
}

