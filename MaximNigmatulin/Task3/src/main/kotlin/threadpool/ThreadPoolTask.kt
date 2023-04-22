package threadpool

import utils.log
import java.util.concurrent.BlockingQueue
import java.util.concurrent.LinkedBlockingQueue

class ThreadPoolTask<TResult>(
    override val name: String,
    private val pool: ThreadPool,
    val function: () -> TResult
) : INamedRunnable {

    @Volatile
    var _result: TResult? = null

    private var childTasks: BlockingQueue<INamedRunnable> = LinkedBlockingQueue()

    fun isCompleted(): Boolean {
        return _result != null
    }

    fun result(): TResult {
        try {
            _result = function()
            return _result!!
        } catch (ex: Exception) {
            throw AggregateException(ex)
        }
    }

    override fun run() {
        try {
            _result = function()
        } finally {
            log("[$name] finished")
            while (!childTasks.isEmpty()) {
                pool.enqueue(childTasks.take())
            }
        }
    }

    fun <TNewResult> continueWith(f: (TResult) -> TNewResult): ThreadPoolTask<TNewResult> {
        val newTask = ThreadPoolTask("$name+", pool) { f(result()) }

        if (isCompleted()) {
            pool.enqueue(newTask)
        } else {
            childTasks.put(newTask)
        }

        return newTask
    }
}
