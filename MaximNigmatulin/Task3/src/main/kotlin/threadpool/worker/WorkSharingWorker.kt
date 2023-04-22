package threadpool.worker

import threadpool.INamedRunnable
import utils.log
import java.util.*
import kotlin.collections.HashMap

class WorkSharingWorker(
    val queueMap: HashMap<Long, Deque<INamedRunnable>>,
    val tokenIsExpired: () -> Boolean,
) : Thread() {

    val THRESHOLD = 2

    override fun run() {
        while (!tokenIsExpired()) {
            val me = currentThread().id

            val task: INamedRunnable?
            synchronized(queueMap[me]!!) {
                task = queueMap[me]!!.poll()
            }

            task?.let {
                log("<${task.name}> is running...")
                task.run()
            }

            val size = queueMap[me]!!.size
            if ((0..size).random() == size) {
                val victim = queueMap.keys.random()

                val min = if (victim < me) victim else me
                val max = if (victim < me) me else victim

                synchronized(queueMap[min]!!) {
                    synchronized(queueMap[max]!!) {
                        log("initiated work sharing:")
                        log("before sharing sizes: (${queueMap[min]!!.size}, ${queueMap[max]!!.size})")
                        balance(queueMap[min]!!, queueMap[max]!!)
                        log("after sharing sizes: (${queueMap[min]!!.size}, ${queueMap[max]!!.size})")
                    }
                }
            }
        }
        println("cToken expired")
        interrupt()
    }

    private fun balance(q0: Queue<INamedRunnable>, q1: Queue<INamedRunnable>) {
        val qMin = if (q0.size < q1.size) q0 else q1
        val qMax = if (q0.size < q1.size) q1 else q0
        val diff = qMax.size - qMin.size
        if (diff > THRESHOLD) while (qMax.size > qMin.size) qMin.add(qMax.poll())
    }
}
