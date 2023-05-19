package ershov;

import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

import ershov.examsystem.ExamSystemLazySet;
import ershov.examsystem.ExamSystemOptimisticSet;
import ershov.examsystem.IExamSystem;

@RestController
public class ExamSystemController {

	private final IExamSystem examSystemOptimisticSet = new ExamSystemOptimisticSet();
	private final IExamSystem examSystemLazySet = new ExamSystemLazySet();

	@RequestMapping("/v1/add")
	public void examSystemAddV1(@RequestParam(value = "student") long student,
			@RequestParam(value = "course") long course) {
		examSystemOptimisticSet.add(student, course);
	}

	@RequestMapping("/v2/add")
	public void examSystemAddV2(@RequestParam(value = "student") long student,
			@RequestParam(value = "course") long course) {
		examSystemLazySet.add(student, course);
	}

	@RequestMapping("/v1/remove")
	public void examSystemRemoveV1(@RequestParam(value = "student") long student,
			@RequestParam(value = "course") long course) {
		examSystemOptimisticSet.remove(student, course);
	}

	@RequestMapping("/v2/remove")
	public void examSystemRemoveV2(@RequestParam(value = "student") long student,
			@RequestParam(value = "course") long course) {
		examSystemLazySet.remove(student, course);
	}

	@RequestMapping("/v1/contains")
	public boolean examSystemContainsV1(@RequestParam(value = "student") long student,
			@RequestParam(value = "course") long course) {
		return examSystemOptimisticSet.contains(student, course);
	}

	@RequestMapping("/v2/contains")
	public boolean examSystemContainsV2(@RequestParam(value = "student") long student,
			@RequestParam(value = "course") long course) {
		return examSystemLazySet.contains(student, course);
	}

	@RequestMapping("/v1/count")
	public int examSystemCountV1() {
		return examSystemOptimisticSet.count();
	}

	@RequestMapping("/v2/count")
	public int examSystemCountV2() {
		return examSystemLazySet.count();
	}

}
