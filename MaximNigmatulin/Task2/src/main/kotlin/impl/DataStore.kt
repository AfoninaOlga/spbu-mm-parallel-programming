package pc.impl

import pc.interfaces.DataStoreProtocol
import java.util.*
import java.util.concurrent.locks.ReentrantLock
import kotlin.concurrent.withLock

class DataStore<T> : DataStoreProtocol<T> {
    private val queue: Queue<T> = LinkedList()
    private val lock = ReentrantLock()
    private val emptyQueueCondition = lock.newCondition()

    override fun push(obj: T) {
        lock.withLock {
            this.queue.add(obj)
            emptyQueueCondition.signal()
        }
    }

    private fun waitOnEmpty() {
        while (queue.isEmpty()) {
            emptyQueueCondition.await()
        }
    }

    override fun pop(): T {
        lock.withLock {
            waitOnEmpty()
            return queue.remove()
        }
    }

    override fun expose(): List<T> {
        return queue.toList()
    }
}