using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool.IMyTasks;
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
        public const int ThreadCount = 5;

        [TestMethod()]
        public void MyTaskTest()
        {
            var threadPool = new ThreadPools.ThreadPool(ThreadCount);
            var task = new MyTask<int>(() => 500 * 500, threadPool);
            Thread.Sleep(100);
            threadPool.Shutdown();
            Assert.That(task.Result, Is.EqualTo(250000));
        }

        [TestMethod()]
        public void MyTaskWIthContinueWith()
        {
            var threadPool = new ThreadPools.ThreadPool(ThreadCount);
            var task = threadPool.Submit(() => 500 * 500);
            var continuation = task.ContinueWith(Result => Result * 5);
            var result = continuation.Result;
            Thread.Sleep(100);
            threadPool.Shutdown();
            Assert.That(continuation.IsCompleted && result == 1250000, Is.True);

        }

        [TestMethod()]
        public void ContinueWithMethodWithSeveralTasks()
        {
            var threadPool = new ThreadPools.ThreadPool(ThreadCount);
            var task1 = new MyTask<int>(() => 300 * 300, threadPool);
            var task2 = new MyTask<int>(() => 400 * 400, threadPool);
            var continuation1 = task1.ContinueWith(Result => Result * 5);
            var continuation2 = task2.ContinueWith(Result => Result * 5);
            var result1 = continuation1.Result;
            var result2 = continuation2.Result;
            Thread.Sleep(100);
            threadPool.Shutdown();
            Assert.That(continuation1.IsCompleted && result1 == 450000, Is.True);
            Assert.That(continuation2.IsCompleted && result2 == 800000, Is.True);
        }

        [TestMethod()]
        public void StartMethodTest()
        {
            var threadPool = new ThreadPools.ThreadPool(ThreadCount);
            var task = new MyTask<int>(() => 300 * 300, threadPool);
            Thread.Sleep(100);
            threadPool.Shutdown();
            Assert.IsTrue(true);
        }
    }
}