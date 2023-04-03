package pc.impl

import java.util.Queue
import java.util.LinkedList
import java.util.concurrent.locks.ReentrantLock
import kotlin.concurrent.withLock

class DataStore<T> {
    private val queue: Queue<T> = LinkedList()
    private val lock = ReentrantLock()
    private val emptyQueueCondition = lock.newCondition()

    fun push(obj: T) {
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

    fun pop(): T {
        lock.withLock {
            waitOnEmpty()
            return queue.remove()
        }
    }

    fun expose(): List<T> {
        return queue.toList()
    }
}