namespace Task4
{
	public class ExamSystem : IExamSystem
	{
		private readonly ISet<ExamInfo> set;

		public ExamSystem(Func<ISet<ExamInfo>> setFactory)
		{
			set = setFactory();
		}

		public int Count => set.Count;

		public void Add(long studentId, long courseId) => set.Add(new ExamInfo(studentId, courseId));

		public bool Contains(long studentId, long courseId) => set.Contains(new ExamInfo(studentId, courseId));

		public void Remove(long studentId, long courseId) => set.Remove(new ExamInfo(studentId, courseId));
	}
}
