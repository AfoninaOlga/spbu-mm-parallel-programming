package ershov.examsystem;

import java.util.HashSet;
import java.util.Map;
import java.util.Set;

public class ExamSystem implements IExamSystem {

	private final Set<Map.Entry<Long, Long>> examResults = new HashSet<>();

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
