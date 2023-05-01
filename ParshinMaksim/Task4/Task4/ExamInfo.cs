namespace Task4
{
	public readonly record struct ExamInfo(long StudentId, long ExamId) : IComparable<ExamInfo>
	{
		public int CompareTo(ExamInfo other)
		{
			var studentCompareResult = StudentId.CompareTo(other.StudentId);

			if (studentCompareResult == 0)
			{
				return ExamId.CompareTo(other.ExamId);
			}

			return studentCompareResult;
		}
	}
}
