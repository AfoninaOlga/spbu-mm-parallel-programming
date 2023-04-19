using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using ProducerConsumer.DataVerification;
using ProducerConsumer.ILocks;
using ProducerConsumer.Producers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assert = NUnit.Framework.Assert;

namespace ProducerConsumer.Producers.Tests
{
    [TestClass()]
    public class ProducerTests
    {
        [TestMethod()]
        public void ProduceTest()
        {
            var tasks = new List<DataContainer<int>>();
            var locker = new TTASLock();
            var producer = new Producers.Producer(locker, tasks);
            Thread.Sleep(100);
            producer.Join();

            Assert.That(tasks.Count, Is.EqualTo(1));
        }

        [TestMethod()]
        public void RunThreadTest()
        {
            Thread thread = new Thread(new ThreadStart(Sum));
            thread.Start();

            Assert.IsTrue(thread.IsAlive);
        }

        static void Sum()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Worker thread: {0}", i);
                Thread.Sleep(100);
            }
        }
    }
}