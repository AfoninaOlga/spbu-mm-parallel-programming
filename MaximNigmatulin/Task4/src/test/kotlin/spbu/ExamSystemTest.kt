package spbu

import kotlinx.coroutines.launch
import kotlinx.coroutines.runBlocking
import org.junit.jupiter.params.ParameterizedTest
import org.junit.jupiter.params.provider.MethodSource
import spbu.models.StudentRecord
import spbu.system.ExamSystem
import spbu.system.createExamSystem
import kotlin.test.assertEquals

class ExamSystemTest {

    @ParameterizedTest
    @MethodSource("examSystemProvider")
    fun `test set interface functions`(es: ExamSystem) {
        val storage = createExamSystem(es)

        val r1 = StudentRecord(1, 1)
        val r2 = StudentRecord(2, 2)
        val r3 = StudentRecord(1, 2)
        val r4 = StudentRecord(1, 2)

        storage.add(r1)
        storage.add(r2)
        storage.add(r3)
        storage.add(r4)

        assertEquals(3, storage.count())
        for (item in listOf(r1, r2, r3)) {
            assert(storage.contains(item))
        }

        storage.remove(r1)
        storage.remove(r2)
        assertEquals(1, storage.count())
        assert(storage.contains(r3))
    }

    @ParameterizedTest
    @MethodSource("examSystemProvider")
    fun `sequential blind comparison against built-in set`(es: ExamSystem) {
        val storage = createExamSystem(es)
        val builtin = mutableSetOf<StudentRecord>()

        val valuesToAdd = 1000
        val valuesToTryDelete = 1000
        val range = 1000

        (0 until valuesToAdd).forEach { _ ->
            val record = StudentRecord((0..range).random().toLong(), (0..range).random().toLong())
            storage.add(record)
            builtin.add(record)
        }

        (0 until valuesToTryDelete).forEach { _ ->
            val record = StudentRecord((0..range).random().toLong(), (0..range).random().toLong())
            storage.remove(record)
            builtin.remove(record)
        }

        assertEquals(builtin.size, storage.count())
        for (item in builtin) {
            storage.contains(item)
        }
    }

    @ParameterizedTest
    @MethodSource("examSystemProvider")
    fun `concurrent blind comparison against built-in set`(es: ExamSystem) {
        val storage = createExamSystem(es)
        val builtin = mutableSetOf<StudentRecord>()

        val coroutinesCount = 100
        val valuesToAddForCoroutine = 100
        val valuesToTryDeleteForCoroutine = 100
        val range = 100

        runBlocking {
            for (i in 0 until coroutinesCount) {
                launch {
                    (0 until valuesToAddForCoroutine).forEach { _ ->
                        val record = StudentRecord((0..range).random().toLong(), (0..range).random().toLong())
                        storage.add(record)
                        builtin.add(record)
                    }

                    (0 until valuesToTryDeleteForCoroutine).forEach { _ ->
                        val record = StudentRecord((0..range).random().toLong(), (0..range).random().toLong())
                        storage.remove(record)
                        builtin.remove(record)
                    }
                }
            }
        }

        assertEquals(builtin.size, storage.count())
        for (item in builtin) {
            storage.contains(item)
        }
    }

    companion object {
        @JvmStatic
        fun examSystemProvider() = listOf(ExamSystem.FINE_GRAINED, ExamSystem.LAZY)
    }
}
