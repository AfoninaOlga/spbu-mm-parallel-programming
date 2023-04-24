namespace ThreadPool.Tests
{
    public class Tests
    {
        [Test]
        public void SmokeStealingTest()
        {
            using (MyThreadPool threadPool = new(1))
            {
                var task = new MyTask<int>(() => 1, threadPool);
                task.Start();
                Assert.That(task.Result, Is.EqualTo(1));
            }
        }

        [Test]
        public void SmokeSharingTest()
        {
            using (MyThreadPool threadPool = new(1, ThreadPoolWorkStrategy.Sharing))
            {
                var task = new MyTask<int>(() => 1, threadPool);
                task.Start();
                Assert.That(task.Result, Is.EqualTo(1));
            }
        }

        [Test]
        public void TasksMoreThanThreadsStealingTest()
        {
            using (MyThreadPool threadPool = new(3))
            {
                var tasksSize = 15;
                var tasks = new MyTask<int>[tasksSize];
                for (var i = 0; i < tasksSize; ++i)
                {
                    tasks[i] = new MyTask<int>(() => { Thread.Sleep(100); return 2 + 2; }, threadPool);
                }

                for (var i = 0; i < tasksSize; ++i)
                {
                    tasks[i].Start();
                }

                for (var i = 0; i < tasksSize; ++i)
                {
                    Assert.That(tasks[i].Result, Is.EqualTo(4));
                }
            }
        }

        [Test]
        public void TasksMoreThanThreadsSharingTest()
        {
            using (MyThreadPool threadPool = new(3, ThreadPoolWorkStrategy.Sharing))
            {
                var tasksSize = 15;
                var tasks = new MyTask<int>[tasksSize];
                for (var i = 0; i < tasksSize; ++i)
                {
                    tasks[i] = new MyTask<int>(() => { Thread.Sleep(100); return 2 + 2; }, threadPool);
                }

                for (var i = 0; i < tasksSize; ++i)
                {
                    tasks[i].Start();
                }

                for (var i = 0; i < tasksSize; ++i)
                {
                    Assert.That(tasks[i].Result, Is.EqualTo(4));
                }
            }
        }

        [Test]
        public void ContinueWithStealingTest()
        {
            using (MyThreadPool threadPool = new(4))
            {
                var task = new MyTask<bool>(() => 5 > 4, threadPool);
                var taskContinued1 = task.ContinueWith(x => { if (x) return 10; else return -5; });
                var taskContinued2 = task.ContinueWith(x => { if (x) return 5 * 3; else return 1 + 1; });
                var taskContinued1Continued = taskContinued1.ContinueWith(x => { if (x == 10) return 1; else return 0; });
                task.Start();
                Assert.Multiple(() =>
                {
                    Assert.That(task.Result, Is.EqualTo(true));
                    Assert.That(taskContinued1.Result, Is.EqualTo(10));
                    Assert.That(taskContinued2.Result, Is.EqualTo(15));
                    Assert.That(taskContinued1Continued.Result, Is.EqualTo(1));
                });
            }
        }

        [Test]
        public void ContinueWithSharingTest()
        {
            using (MyThreadPool threadPool = new(4, ThreadPoolWorkStrategy.Sharing))
            {
                var task = new MyTask<bool>(() => 5 > 4, threadPool);
                var taskContinued1 = task.ContinueWith(x => { if (x) return 10; else return -5; });
                var taskContinued2 = task.ContinueWith(x => { if (x) return 5 * 3; else return 1 + 1; });
                var taskContinued1Continued = taskContinued1.ContinueWith(x => { if (x == 10) return 1; else return 0; });
                task.Start();
                Assert.Multiple(() =>
                {
                    Assert.That(task.Result, Is.EqualTo(true));
                    Assert.That(taskContinued1.Result, Is.EqualTo(10));
                    Assert.That(taskContinued2.Result, Is.EqualTo(15));
                    Assert.That(taskContinued1Continued.Result, Is.EqualTo(1));
                });
            }
        }

        [Test]
        public void AggregateExceptionContinueWithStealingTest()
        {
            using (MyThreadPool threadPool = new(4))
            {
                var task = new MyTask<bool>(() => throw new Exception(), threadPool);
                var taskContinued1 = task.ContinueWith(x => { if (x) return 10; else return -5; });
                var taskContinued1Continued = taskContinued1.ContinueWith(x => { if (x == 10) return 1; else return 0; });
                task.Start();
                Assert.Multiple(() =>
                {
                    Assert.Throws(Is.TypeOf<AggregateException>(), () => _ = task.Result);
                    Assert.Throws(Is.TypeOf<AggregateException>(), () => _ = taskContinued1.Result);
                    Assert.Throws(Is.TypeOf<AggregateException>(), () => _ = taskContinued1Continued.Result);
                });
            }
        }

        [Test]
        public void AggregateExceptionContinueWithSharingTest()
        {
            using (MyThreadPool threadPool = new(4, ThreadPoolWorkStrategy.Sharing))
            {
                var task = new MyTask<bool>(() => throw new Exception(), threadPool);
                var taskContinued1 = task.ContinueWith(x => { if (x) return 10; else return -5; });
                var taskContinued1Continued = taskContinued1.ContinueWith(x => { if (x == 10) return 1; else return 0; });
                task.Start();
                Assert.Multiple(() =>
                {
                    Assert.Throws(Is.TypeOf<AggregateException>(), () => _ = task.Result);
                    Assert.Throws(Is.TypeOf<AggregateException>(), () => _ = taskContinued1.Result);
                    Assert.Throws(Is.TypeOf<AggregateException>(), () => _ = taskContinued1Continued.Result);
                });
            }
        }

        public void NonLessThanNumberThreadsTest()
        {
            int thredsSize = 4;
            using (MyThreadPool threadPool = new(thredsSize))
            {
                Assert.That(threadPool.ThreadsSize, Is.EqualTo(thredsSize));
            }
        }
    }
}