package threadpool

import extensions.stringify
import utils.log
import java.lang.RuntimeException
import java.util.concurrent.BlockingQueue

class PoolWorker(
    private val queue: BlockingQueue<INamedRunnable>,
    val tokenIsExpired: () -> Boolean
) : Thread() {
    override fun run() {
        while (!tokenIsExpired()) {
            log(queue.stringify())

            val task = queue.take()

            try {
                log("<${task.name}> is running...")
                task.run()
            } catch (e: RuntimeException) {
                println("Thread pool is interrupted due to an issue: " + e.message)
            }
        }
        println("cToken expired")
        interrupt()
    }
}
