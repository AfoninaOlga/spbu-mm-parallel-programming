package spbu.system

enum class ExamSystem(val value: String) {
    FINE_GRAINED("fine-grained"),
    LAZY("lazy")
}

val fineGrainedStorage = createExamSystem(ExamSystem.FINE_GRAINED)
val lazyStorage = createExamSystem(ExamSystem.LAZY)
