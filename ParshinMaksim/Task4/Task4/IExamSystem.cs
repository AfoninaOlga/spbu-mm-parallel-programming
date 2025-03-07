﻿namespace Task4
{
	public interface IExamSystem
	{
		void Add(long studentId, long courseId);

		void Remove(long studentId, long courseId);

		bool Contains(long studentId, long courseId);

		int Count { get; }
	}
}
