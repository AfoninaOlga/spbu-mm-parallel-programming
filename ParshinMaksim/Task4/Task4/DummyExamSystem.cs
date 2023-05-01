namespace Task4
{
	public class DummyExamSystem : IExamSystem
	{
		private readonly HashSet<(long, long)> values = new HashSet<(long, long)>();
		public int Count => values.Count;

		public void Add(long studentId, long courseId) => values.Add((studentId, courseId));

		public bool Contains(long studentId, long courseId) => values.Contains((studentId, courseId));

		public void Remove(long studentId, long courseId) => values.Remove((studentId, courseId));
	}
}
