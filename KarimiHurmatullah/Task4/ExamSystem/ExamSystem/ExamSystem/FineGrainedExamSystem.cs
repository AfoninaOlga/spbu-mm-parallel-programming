using ExamSystem.Collections;
using ExamSystem.Interfaces;

namespace ExamSystem.ExamSystem
{
    public class FineGrainedExamSystem : IExamSystem
    {
        private static readonly FineGrainedSet<(long, long)> fineGrained = new();

        public void Add(long studentId, long courseId)
        {
            fineGrained.Add((studentId, courseId));
        }

        public void Remove(long studentId, long courseId)
        {
            fineGrained.Remove((studentId, courseId));
        }

        public bool Contains(long studentId, long courseId)
        {
            return fineGrained.Contains((studentId, courseId));
        }

        public int Count => fineGrained.Count;
    }
}
