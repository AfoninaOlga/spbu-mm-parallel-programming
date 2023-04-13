package pc.mechanism

import java.util.Queue
import java.util.LinkedList
import java.util.concurrent.locks.ReentrantLock
import kotlin.concurrent.withLock


class DataStore<T>(val log: (String) -> Unit = {}) {
    private val queue: Queue<T> = LinkedList()
    private val lock = ReentrantLock()

    fun push(obj: T) {
        log("trying to push $obj")
        while (!lock.tryLock()) {
        }

        queue.add(obj)
        val snapshot = expose()
        log("added $obj -> $snapshot")
        lock.unlock()
    }

    fun pop(): T? {
        log("trying to pop")
        while (!lock.tryLock()) {
        }
        if (queue.isEmpty()) {
            lock.unlock()
            return null
        }
        val toReturn = queue.remove()
        val snapshot = expose()

        log("rm $toReturn -> $snapshot")

        lock.unlock()
        return toReturn
    }

    fun expose(): List<T> {
        return queue.toList()
    }
}
