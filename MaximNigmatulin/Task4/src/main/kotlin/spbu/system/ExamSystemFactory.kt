package spbu.system

import spbu.system.finegrained.FineGrainedSet
import spbu.system.lazyset.LazySet

fun createExamSystem(es: ExamSystem): ExamSystemInterface {
    if (es == ExamSystem.FINE_GRAINED) {
        return FineGrainedSet()
    }
    if (es == ExamSystem.LAZY) {
        return LazySet()
    }
    throw RuntimeException("Not implemented")
}
