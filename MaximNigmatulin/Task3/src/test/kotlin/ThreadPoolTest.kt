import org.junit.jupiter.api.Test
import threadpool.ThreadPoolTask
import threadpool.ThreadPool
import threadpool.WorkStrategy
import kotlin.test.assertEquals

class ThreadPoolTest {
    @Test
    fun `result is counted`() {
        val pool = ThreadPool(2, WorkStrategy.SHARING)

        val simpleTask = ThreadPoolTask("task", pool) {
            2 * 2
        }

        pool.enqueue(simpleTask)

        pool.start()

        assertEquals(simpleTask.result(), 4)
        pool.close()
    }

    @Test
    fun `result is counted with continuations`() {
        val pool = ThreadPool(2, WorkStrategy.SHARING)

        val simpleTask = ThreadPoolTask("task", pool) {
            2 * 2
        }

        val twiceContinued = simpleTask.continueWith {
            it * 2
        }.continueWith {
            it * 2
        }
        pool.enqueue(simpleTask)

        pool.start()

        assertEquals(twiceContinued.result(), 16)
        pool.close()
    }
}