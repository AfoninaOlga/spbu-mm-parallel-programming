using ExamSystem.Collections;
using ExamSystem.Interfaces;

namespace ExamSystem.ExamSystem
{
    public class CoarseExamSystem : IExamSystem
    {
        private static readonly CoarseSet<(long, long)> coarseSet = new();

        public void Add(long studentId, long courseId)
        {
            coarseSet.Add((studentId, courseId));
        }

        public void Remove(long studentId, long courseId)
        {
            coarseSet.Remove((studentId, courseId));
        }

        public bool Contains(long studentId, long courseId)
        {
            return coarseSet.Contains((studentId, courseId));
        }

        public int Count => coarseSet.Count;
    }
}
