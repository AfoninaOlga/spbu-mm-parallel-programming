package threadpool

import java.util.concurrent.BlockingQueue
import java.util.concurrent.LinkedBlockingQueue

class ThreadPool(nThreads: Int) : AutoCloseable {
    private val threads: ArrayList<PoolWorker> = ArrayList()
    private val queue: BlockingQueue<INamedRunnable> = LinkedBlockingQueue()
    private var continuationToken: Boolean = false

    init {
        for (i in 0 until nThreads) {
            threads.add(PoolWorker(queue) { continuationToken })
        }
    }

    fun start() {
        threads.forEach {
            it.start()
        }
    }

    fun enqueue(task: INamedRunnable) {
        queue.put(task)
    }


    override fun close() {
        continuationToken = true
    }
}
