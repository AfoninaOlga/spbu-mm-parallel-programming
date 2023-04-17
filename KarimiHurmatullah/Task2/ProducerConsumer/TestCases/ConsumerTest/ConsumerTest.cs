using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProducerConsumer.Consumer;
using ProducerConsumer.Connector;
using ProducerConsumer.Producer;
using System.Globalization;

namespace TestCases.ConsumerTest
{
    internal class ConsumerTest
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private List<string> buffer = new List<string>();

        private Connector connector = new Connector();

        private Consumer consumer = new Consumer();

        [Test]
        public async Task ConsumeTest()
        {
            var consumerTask = consumer.Consume(buffer, connector, cancellationTokenSource.Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));
            cancellationTokenSource.Cancel();
            await consumerTask.ContinueWith(t => { });

            Assert.True(buffer.Count == 0);
            Assert.True(connector.GetBufferCount() == 0);
        }
    }
}
