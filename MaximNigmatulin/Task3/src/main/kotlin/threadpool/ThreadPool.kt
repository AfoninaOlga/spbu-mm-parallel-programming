package threadpool

import threadpool.worker.WorkSharingWorker
import threadpool.worker.WorkStealingWorker
import java.util.Deque
import java.util.concurrent.LinkedBlockingDeque

class ThreadPool(nThreads: Int, private val workStrategy: WorkStrategy) : AutoCloseable {
    private val queueMap: HashMap<Long, Deque<INamedRunnable>> = hashMapOf()
    private var continuationToken: Boolean = false
    private val threads: Array<Thread>

    init {
        threads =
            (0 until nThreads).map {
                if (workStrategy == WorkStrategy.SHARING)
                    WorkSharingWorker(queueMap) { continuationToken }
                else
                    WorkStealingWorker(queueMap) { continuationToken }
            }.toTypedArray()

        threads.forEach {
            queueMap[it.id] = LinkedBlockingDeque()
        }
    }

    fun start() {
        threads.forEach { it.start() }
    }

    fun enqueue(task: INamedRunnable) {
        val chosenId = threads[0].id

        queueMap[chosenId]?.let {
            synchronized(it) {
                it.add(task)
            }
        }
    }

    override fun close() {
        continuationToken = true
    }
}
