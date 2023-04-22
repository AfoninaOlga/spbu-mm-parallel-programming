import org.junit.jupiter.api.assertThrows
import org.junit.jupiter.api.assertTimeout
import org.junit.jupiter.api.assertTimeoutPreemptively
import org.junit.jupiter.params.ParameterizedTest
import org.junit.jupiter.params.provider.MethodSource
import threadpool.AggregateException
import threadpool.ThreadPoolTask
import threadpool.ThreadPool
import threadpool.WorkStrategy
import java.time.Duration
import java.util.concurrent.ConcurrentSkipListSet
import kotlin.test.assertEquals

class ThreadPoolTest {

    @ParameterizedTest
    @MethodSource("taskNumThreadNumStrategyProvider")
    fun `Test pure tasks-threads configurations completing`(testSupplyTriple: Triple<Int, Int, WorkStrategy>) {
        val (taskNum, threadNum, strategy) = testSupplyTriple
        val pool = ThreadPool(threadNum, strategy)

        val tasks = (0 until taskNum).map {
            val task = ThreadPoolTask(it.toString(), pool) { 2 * 2 }
            pool.enqueue(task)
            task
        }

        pool.start()
        Thread.sleep(100)
        pool.close()

        tasks.forEach { task -> assertEquals(task.result(), 4) }
    }

    @ParameterizedTest
    @MethodSource("taskNumThreadNumStrategyProvider")
    fun `Test continuations completing`(testSupplyTriple: Triple<Int, Int, WorkStrategy>) {
        val (continuationNum, threadNum, strategy) = testSupplyTriple
        val pool = ThreadPool(threadNum, strategy)

        val rootTask = ThreadPoolTask("root", pool) { 0 }
        val tasks = arrayListOf(rootTask)

        (0 until continuationNum).map {
            tasks.add(tasks.last().continueWith { it + 1 })
        }

        pool.enqueue(rootTask)

        pool.start()
        Thread.sleep(100)
        pool.close()

        assertEquals(continuationNum, tasks.last().result())
    }

    @ParameterizedTest
    @MethodSource("threadNumStrategyProvider")
    fun `Test thread count`(testSupplyTuple: Pair<Int, WorkStrategy>) {
        val (nThreads, strategy) = testSupplyTuple

        val pool = ThreadPool(nThreads, strategy)
        val set = ConcurrentSkipListSet<Long>()

        (0 until nThreads * 10).map {
            pool.enqueue(ThreadPoolTask(it.toString(), pool) {
                set.add(Thread.currentThread().id)
            })
        }

        assertTimeoutPreemptively(Duration.ofMillis(200)) {
            pool.start()
            while (set.size != nThreads) {
                Thread.sleep(0)
            }
        }
        pool.close()
    }

    @ParameterizedTest
    @MethodSource("strategyProvider")
    fun `Test pure task throws AggregatedException`(strategy: WorkStrategy) {
        val pool = ThreadPool(2, strategy)
        val exceptionDescription = "Something happened while computing!"

        val throws = ThreadPoolTask("root", pool) {
            throw RuntimeException(exceptionDescription)
        }

        pool.enqueue(throws)

        (0..100).forEach {
            pool.enqueue(ThreadPoolTask(it.toString(), pool) {})
        }

        pool.start()

        val exception = assertThrows<AggregateException> {
            pool.use {
                throws.result()
            }
        }

        assertEquals("Aggregated: ($exceptionDescription)", exception.message)
    }

    @ParameterizedTest
    @MethodSource("strategyProvider")
    fun `Test continued task throws AggregatedException`(strategy: WorkStrategy) {
        val pool = ThreadPool(2, strategy)
        val exceptionDescription = "Something happened while computing!"

        val root = ThreadPoolTask("root", pool) { 2 }

        val throws = root.continueWith {
            throw RuntimeException(exceptionDescription)
            it * 2
        }

        val final = throws.continueWith { it * 2 }
        pool.enqueue(root)

        pool.start()

        val exception = assertThrows<AggregateException> {
            pool.use {
                final.result()
            }
        }

        val wrap: (String) -> String = { "Aggregated: ($it)" }

        assertEquals(wrap(wrap(exceptionDescription)), exception.message)
    }

    companion object {
        @JvmStatic
        fun strategyProvider(): List<WorkStrategy> = listOf(WorkStrategy.SHARING, WorkStrategy.STEALING)

        private fun threadCountProvider(): List<Int> = listOf(1, 2, 5, 8, 20)

        private fun taskCountProvider(): List<Int> = listOf(1, 2, 5, 40, 100)

        @JvmStatic
        fun threadNumStrategyProvider(): List<Pair<Int, WorkStrategy>> =
            cartesianProduct(threadCountProvider(), strategyProvider())

        @JvmStatic
        fun taskNumThreadNumStrategyProvider(): List<Triple<Int, Int, WorkStrategy>> =
            cartesianProductFlattenPairsToTriples(taskCountProvider(), threadNumStrategyProvider())
    }
}


private fun <T, U> cartesianProduct(c1: Collection<T>, c2: Collection<U>): List<Pair<T, U>> {
    return c1.flatMap { lhsElem -> (c2.map { rhsElem -> (lhsElem to rhsElem) }) }
}

private fun <L, F, T, U : Pair<L, F>> cartesianProductFlattenPairsToTriples(
    c1: Collection<T>,
    c2: Collection<U>
): List<Triple<T, L, F>> {
    return c1.flatMap { le -> c2.map { re -> Triple(le, re.first, re.second) } }
}
