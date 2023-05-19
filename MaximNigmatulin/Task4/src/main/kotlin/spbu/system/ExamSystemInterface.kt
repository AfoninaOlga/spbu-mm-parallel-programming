package spbu.system

import spbu.models.StudentRecord

interface ExamSystemInterface {
    fun add(record: StudentRecord): Boolean
    fun remove(record: StudentRecord): Boolean
    fun contains(record: StudentRecord): Boolean
    fun count(): Int
}
