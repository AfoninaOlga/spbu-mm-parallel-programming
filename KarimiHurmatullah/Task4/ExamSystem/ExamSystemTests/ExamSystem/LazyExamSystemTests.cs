using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExamSystem.ExamSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace ExamSystem.ExamSystem.Tests
{
    [TestClass()]
    public class LazyExamSystemTests
    {
        private static readonly LazyExamSystem lazyTest = new();

        [TestMethod()]
        public void AddOneValue()
        {
            lazyTest.Add(1, 1);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void CountOneAddedValue()
        {
            var countedValue = lazyTest.Count;
            if (countedValue == 1)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsFalse(true);
            }
        }

        [TestMethod()]
        public void ContainsOneAddedValue()
        {
            Assert.That(lazyTest.Contains(1, 1), Is.True);
        }

        [TestMethod()]
        public void RemoveOneAddedValue()
        {
            lazyTest.Remove(1, 1);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void MultipleAdding()
        {
            var threadCount = 10;
            var thread = new Thread[threadCount];
            for (var i = 0; i < threadCount; ++i)
            {
                var value1 = i;
                var value2 = i * 3;
                thread[i] = new Thread(() => lazyTest.Add(value1, value2));
                thread[i].Start();
            }

            for (var i = 0; i < threadCount; ++i)
            {
                thread[i].Join();
            }

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void MultipleContains()
        {
            var threadCount = 10;
            var thread = new Thread[threadCount];
            for (var i = 0; i < threadCount; ++i)
            {
                var value1 = i;
                var value2 = i * 3;
                thread[i] = new Thread(() => lazyTest.Contains(value1, value2));
                thread[i].Start();
            }

            for (var i = 0; i < threadCount; ++i)
            {
                thread[i].Join();
            }

            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void MultipleRemove()
        {
            var threadCount = 10;
            var thread = new Thread[threadCount];
            for (var i = 0; i < threadCount; i++)
            {
                var value1 = i;
                var value2 = i * 3;
                thread[i] = new Thread(() => lazyTest.Remove(value1, value2));
                thread[i].Start();
            }

            for (var i = 0; i < threadCount; ++i)
            {
                thread[i].Join();
            }

            Assert.IsTrue(true);
        }
    }
}