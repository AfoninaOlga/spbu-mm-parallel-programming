using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using ProducerConsumer.Consumers;
using ProducerConsumer.ILocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;
using ProducerConsumer.DataVerification;

namespace ProducerConsumer.Consumers.Tests
{
    [TestClass()]
    public class ConsumerTests
    {
        [TestMethod()]
        public void ConsumerTest()
        {
            var tasks = new List<DataContainer<int>>();
            var locker = new TTASLock();
            var producer = new Consumer(locker, tasks);
            Thread.Sleep(100);
            producer.Join();

            Assert.That(tasks.Count, Is.EqualTo(0));
        }

        [TestMethod()]
        public void RunThreadTest()
        {
            Thread thread = new Thread(new ThreadStart(Minus));
            thread.Start();

            Assert.IsTrue(thread.IsAlive);
        }

        static void Minus()
        {
            for (int i = 5; i <= 0; i--)
            {
                Console.WriteLine("Worker thread: {0}", i);
                Thread.Sleep(100);
            }
        }
    }
}