using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool.ThreadPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using ThreadPool.IMyTasks;

namespace ThreadPool.ThreadPools.Tests
{
    [TestClass()]
    public class ThreadPoolTests
    {
        public const int ThreadCount = 5;
        public Thread[] _threads;
        public bool IsTerminated = false;

        [TestMethod()]
        public void SubmitTest()
        {
            var threadPool = new ThreadPool(ThreadCount);
            var task = threadPool.Submit(() => 500 * 500);
            task.Start();
            Thread.Sleep(100);
            threadPool.Shutdown();
            Assert.That(task.IsCompleted && task.Result == 250000, Is.True);
        }

        [TestMethod()]
        public void VerifyNThreads()
        {
            var threadPool = new ThreadPool(ThreadCount);
            var tasks = new List<MyTask<int>>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(threadPool.Submit(GetThreadId));
            }
            var selectThreads = tasks.Select(task => task.Result);
            Thread.Sleep(100);
            threadPool.Shutdown();
            var threadIdCount = selectThreads.Count();
            Assert.That(threadIdCount, Is.EqualTo(ThreadCount));
        }

        [TestMethod()]
        public static int GetThreadId()
        {
            var currentThreadId = System.Environment.CurrentManagedThreadId;
            return currentThreadId;
        }

        [TestMethod()]
        public void ShutdownTest()
        {
            var threadPool = new ThreadPool(ThreadCount);
            var cancellationToken = new CancellationTokenSource();
            if (IsTerminated)
            {
                cancellationToken.Cancel();
                for (var i = 0; i < ThreadCount; ++i)
                {
                    _threads[i].Join();
                }
            }
            Thread.Sleep(100);
            threadPool.Shutdown();
            Assert.IsTrue(true);
        }
    }
}