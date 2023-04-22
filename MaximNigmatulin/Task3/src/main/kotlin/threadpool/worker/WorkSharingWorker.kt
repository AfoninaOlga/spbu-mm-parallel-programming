package threadpool.worker

import extensions.stringify
import threadpool.INamedRunnable
import utils.log
import java.util.*
import kotlin.collections.HashMap

class WorkSharingWorker(
    private val queueMap: HashMap<Long, Deque<INamedRunnable>>,
    val tokenIsExpired: () -> Boolean,
) : Thread() {

    private val threshold = 2

    override fun run() {
        while (!tokenIsExpired()) {
            val me = currentThread().id

            val task: INamedRunnable?
            synchronized(queueMap[me]!!) {
                task = queueMap[me]!!.poll()
            }

            task?.let {
                log("${queueMap[me]!!.stringify()} [${task.name}] is running...")
                task.run()
            }

            val size = queueMap[me]!!.size
            if ((0..size).random() == size) {
                val victim = queueMap.keys.random()

                val min = if (victim < me) victim else me
                val max = if (victim < me) me else victim

                synchronized(queueMap[min]!!) {
                    synchronized(queueMap[max]!!) {
                        balance(queueMap[min]!!, queueMap[max]!!)
                    }
                }
            }
        }
        log("cToken expired")
        interrupt()
    }

    private fun balance(q0: Queue<INamedRunnable>, q1: Queue<INamedRunnable>) {
        val qMin = if (q0.size < q1.size) q0 else q1
        val qMax = if (q0.size < q1.size) q1 else q0
        val diff = qMax.size - qMin.size
        if (diff > threshold) while (qMax.size > qMin.size) qMin.add(qMax.poll())
    }
}
