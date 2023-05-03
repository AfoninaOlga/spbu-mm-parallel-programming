using ExamSystem.Tables;

namespace ExamSystem.Systems;

public class FineGrainedSystem : IExamSystem
{
    private readonly LazyTable<Tuple<long, long>> _table = new();

    public void Add(long studentId, long courseId)
    {
        _table.Add(new Tuple<long, long>(studentId, courseId));
    }

    public void Remove(long studentId, long courseId)
    {
        _table.Remove(new Tuple<long, long>(studentId, courseId));
    }

    public bool Contains(long studentId, long courseId)
    {
        return _table.Contains(new Tuple<long, long>(studentId, courseId));
    }

    public int Count => _table.Count();
}