using Task4;

namespace Task4Tests
{
	[TestFixture]
	public class LazySetTests
	{
		public static object[] TestCaseSource =
		{
			new object[] { 1, () => new LazySet<ExamInfo>() },
			new object[] { 10, () => new LazySet<ExamInfo>() },
			new object[] { 20, () => new LazySet<ExamInfo>() },
			new object[] { 50, () => new LazySet<ExamInfo>() },
			new object[] { 100, () => new LazySet<ExamInfo>() },
			new object[] { 1, () => new FineGrainedSet<ExamInfo>() },
			new object[] { 10, () => new FineGrainedSet<ExamInfo>() },
			new object[] { 20, () => new FineGrainedSet<ExamInfo>() },
			new object[] { 50, () => new FineGrainedSet<ExamInfo>() },
			new object[] { 100, () => new FineGrainedSet<ExamInfo>() },
		};

		[TestCaseSource(nameof(TestCaseSource))]
		public void ContainsAddedElementTest(int threadCount, Func<Task4.ISet<ExamInfo>> setFactory)
		{
			var system = new ExamSystem(setFactory);
			system.Add(0, 0);
			Assert.That(system.Contains(0, 0), Is.True);
		}

		[TestCaseSource(nameof(TestCaseSource))]
		public void NotContainsRemovedElementTest(int threadCount, Func<Task4.ISet<ExamInfo>> setFactory)
		{
			var system = new ExamSystem(() => new LazySet<ExamInfo>());
			system.Add(5, 5);
			system.Remove(5, 5);
			Assert.That(system.Contains(5, 5), Is.False);
		}

		[TestCaseSource(nameof(TestCaseSource))]
		public void MultipleElementsTest(int threadCount, Func<Task4.ISet<ExamInfo>> setFactory)
		{
			var system = new ExamSystem(setFactory);

			for (var i = 0; i < 100; i++)
			{
				system.Add(i, i * 15);
			}

			for (var i = 0; i < 100; i++)
			{
				Assert.That(system.Contains(i, i * 15), Is.True);
			}

			for (var i = 0; i < 100; i++)
			{
				system.Remove(i, i * 15);
			}

			for (var i = 0; i < 100; i++)
			{
				Assert.That(system.Contains(i, i * 15), Is.False);
			}
		}

		[TestCaseSource(nameof(TestCaseSource))]
		public void CountTest(int threadCount, Func<Task4.ISet<ExamInfo>> setFactory)
		{
			var system = new ExamSystem(setFactory);

			for (var i = 0; i < 100; i++)
			{
				system.Add(i, i * 15);
			}

			Assert.That(system.Count, Is.EqualTo(100));
		}

		[TestCaseSource(nameof(TestCaseSource))]
		public void ConcurrentAddTest(int threadCount, Func<Task4.ISet<ExamInfo>> setFactory)
		{
			var system = new ExamSystem(setFactory);

			var threads = new Thread[threadCount];

			for (var i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(() => system.Add(5, 5));
			}

			for (var i = 0; i < threadCount; i++)
			{
				threads[i].Start();
			}

			for (var i = 0; i < threadCount; i++)
			{
				threads[i].Join();
			}

			Assert.That(system.Contains(5, 5), Is.True);
			Assert.That(system.Count, Is.EqualTo(1));
		}

		[TestCaseSource(nameof(TestCaseSource))]
		public void ConcurrentAddAndRemoveTest(int threadCount, Func<Task4.ISet<ExamInfo>> setFactory)
		{
			var system = new ExamSystem(setFactory);

			var threads = new Thread[threadCount];

			for (var i = 0; i < threadCount; i++)
			{
				threads[i] = new Thread(() =>
				{
					system.Add(i, i);
					system.Remove(i, i);
				});
			}

			for (var i = 0; i < threadCount; i++)
			{
				threads[i].Start();
			}

			for (var i = 0; i < threadCount; i++)
			{
				threads[i].Join();
			}

			Assert.That(system.Count, Is.EqualTo(0));
		}
	}
}
