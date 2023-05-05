package ershov;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import ershov.examsystem.ExamSystem;
import ershov.examsystem.IExamSystem;

@RestController
public class ExamSystemController {

	private final IExamSystem examSystem = new ExamSystem();

	@RequestMapping("/add")
	public void examSystemAdd(@RequestParam(value = "student") long student,
			@RequestParam(value = "exam") long exam) {
		examSystem.add(student, exam);
	}

	@RequestMapping("/remove")
	public void examSystemRemove(@RequestParam(value = "student", required = false, defaultValue = "12") long student,
			@RequestParam(value = "exam", required = true, defaultValue = "21") long exam) {
		examSystem.remove(student, exam);
	}

	@RequestMapping("/contains")
	public boolean examSystemContains(
			@RequestParam(value = "student") long student,
			@RequestParam(value = "exam") long exam) {
		return examSystem.contains(student, exam);
	}

	@RequestMapping("/count")
	public long examSystemCount() {
		return examSystem.count();
	}

}
