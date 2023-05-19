package ershov.examsystem;

public interface IExamSystem {

	public void add(long studentId, long courseId);

	public void remove(long studentId, long courseId);

	public boolean contains(long studentId, long courseId);

	public int count();

}
