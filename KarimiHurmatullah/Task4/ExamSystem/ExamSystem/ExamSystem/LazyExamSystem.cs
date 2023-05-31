using ExamSystem.Collections;
using ExamSystem.Interfaces;

namespace ExamSystem.ExamSystem
{
    public class LazyExamSystem : IExamSystem
    {
        private static readonly LazySet<(long, long)> lazySet = new();

        public void Add(long studentId, long courseId)
        {
            lazySet.Add((studentId, courseId));
        }

        public void Remove(long studentId, long courseId)
        {
            lazySet.Remove((studentId, courseId));
        }

        public bool Contains(long studentId, long courseId)
        {
            return lazySet.Contains((studentId, courseId));
        }

        public int Count => lazySet.Count;
    }
}
