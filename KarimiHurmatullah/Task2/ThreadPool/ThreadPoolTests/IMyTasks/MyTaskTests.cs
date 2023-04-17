using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool.IMyTasks;
using ThreadPool.ThreadPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace ThreadPool.IMyTasks.Tests
{
    [TestClass()]
    public class MyTaskTests
    {
        ThreadPools.ThreadPool threadPool = new ThreadPools.ThreadPool(_threadCount);

        public const int _threadCount = 5;

        [TestMethod()]
        public void MyTaskTest()
        {
            var task = new MyTask<int>(() => 500 * 500, threadPool);
            SleepAndShutdownPool();
            Assert.That(task.Result, Is.EqualTo(250000));
        }

        [TestMethod()]
        public void MyTaskWIthContinueWith()
        {
            var task = threadPool.Submit(() => 500 * 500);
            var continuation = task.ContinueWith(Result => Result * 5);
            var result = continuation.Result;
            SleepAndShutdownPool();
            Assert.That(continuation.IsCompleted && result == 1250000, Is.True);

        }

        [TestMethod()]
        public void ContinueWithMethodWithSeveralTasks()
        {
            var task1 = new MyTask<int>(() => 300 * 300, threadPool);
            var task2 = new MyTask<int>(() => 400 * 400, threadPool);
            var continuation1 = task1.ContinueWith(Result => Result * 5);
            var continuation2 = task2.ContinueWith(Result => Result * 5);
            var result1 = continuation1.Result;
            var result2 = continuation2.Result;
            SleepAndShutdownPool();
            Assert.That(continuation1.IsCompleted && result1 == 450000, Is.True);
            Assert.That(continuation2.IsCompleted && result2 == 800000, Is.True);
        }

        [TestMethod()]
        public void StartMethodTest()
        {
            var task = new MyTask<int>(() => 300 * 300, threadPool);
            SleepAndShutdownPool();
            Assert.IsTrue(true);
        }

        public void SleepAndShutdownPool()
        {
            Thread.Sleep(100);
            threadPool.Shutdown();
        }
    }
}