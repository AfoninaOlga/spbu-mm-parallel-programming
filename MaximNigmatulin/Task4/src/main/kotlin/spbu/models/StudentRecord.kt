package spbu.models

data class StudentRecord(val studentId: Long, val courseId: Long) {
    override fun hashCode(): Int {
        return (65537 * studentId + courseId.hashCode()).toInt()
    }

    override fun equals(other: Any?): Boolean {
        if (this === other) return true
        if (javaClass != other?.javaClass) return false

        other as StudentRecord

        if (studentId != other.studentId) return false
        if (courseId != other.courseId) return false

        return true
    }
}
