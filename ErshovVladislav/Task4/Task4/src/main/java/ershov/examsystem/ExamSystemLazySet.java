package ershov.examsystem;

import java.util.Map;
import syncronizedset.ISynchronizedSet;
import syncronizedset.LazySet;

public class ExamSystemLazySet implements IExamSystem {

	private final ISynchronizedSet<Map.Entry<Long, Long>> examResults = new LazySet<>();

	@Override
	public void add(long studentId, long courseId) {
		examResults.add(Map.entry(studentId, courseId));
	}

	@Override
	public void remove(long studentId, long courseId) {
		examResults.remove(Map.entry(studentId, courseId));
	}

	@Override
	public boolean contains(long studentId, long courseId) {
		return examResults.contains(Map.entry(studentId, courseId));
	}

	@Override
	public int count() {
		return examResults.size();
	}

}
