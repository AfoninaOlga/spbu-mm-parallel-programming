package threadpool.worker

import extensions.stringify
import threadpool.INamedRunnable
import utils.log
import java.util.Deque
import kotlin.collections.HashMap

class WorkStealingWorker(
    private val queueMap: HashMap<Long, Deque<INamedRunnable>>,
    val tokenIsExpired: () -> Boolean,
) : Thread() {
    override fun run() {
        val me = currentThread().id
        var task: INamedRunnable? = queueMap[me]!!.pollLast()
        while (!tokenIsExpired()) {
            while (task != null) {
                log("${queueMap[me]!!.stringify()} ${task.name} started...")
                task.run()
                task = queueMap[me]!!.pollLast()
            }
            while ((task == null) and !tokenIsExpired()) {
                yield()
                val victim = queueMap.keys.random()
                if (!queueMap[victim]!!.isEmpty()) {
                    task = queueMap[victim]!!.pollFirst()
                    log("tried to steal $victim's task: ${task?.name}")
                }
            }
        }
        log("cToken expired")
        interrupt()
    }
}
