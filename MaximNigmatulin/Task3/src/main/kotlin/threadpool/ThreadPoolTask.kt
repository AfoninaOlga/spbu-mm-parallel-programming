package threadpool

import utils.log
import java.util.concurrent.BlockingQueue
import java.util.concurrent.LinkedBlockingQueue

class ThreadPoolTask<TResult>(
    override val name: String,
    private val pool: ThreadPool,
    val function: () -> TResult
) : INamedRunnable {
    private var _result: TResult? = null

    @Volatile
    private var isCompleted: Boolean = false

    private var childTasks: BlockingQueue<INamedRunnable> = LinkedBlockingQueue()

    fun result(): TResult {
        while (!isCompleted) {
        }
        return _result!!
    }

    override fun run() {
        try {
            _result = function()
        } catch (ex: Exception) {
            throw AggregateException(ex)
        } finally {
            log("<$name> finished")
            isCompleted = true
            while (!childTasks.isEmpty()) {
                pool.enqueue(childTasks.take())
            }
        }
    }

    fun <TNewResult> continueWith(f: (TResult) -> TNewResult): ThreadPoolTask<TNewResult> {
        val newTask = ThreadPoolTask("$name+", pool) {
            val parentResult = this.result()
            f(parentResult)
        }

        if (isCompleted) {
            pool.enqueue(newTask)
        } else {
            childTasks.put(newTask)
        }

        return newTask
    }
}