package pc.impl

import java.util.Queue
import java.util.LinkedList
import java.util.concurrent.Semaphore

class DataStore<T>(val log: (String) -> Unit = {}) {
    private val queue: Queue<T> = LinkedList()
    private val lock = Semaphore(1)
    private val emptyQueueLock = Semaphore(1)

    init {
        emptyQueueLock.acquire()
    }

    fun push(obj: T) {
        log("waiting to get permit to PUSH")
        lock.acquire()
        log("locked to push $obj")
        queue.add(obj)
        val snapshot = expose()
        log("added $obj -> $snapshot")
        log("unlock")

        lock.release()
        emptyQueueLock.release()
    }

    fun pop(): T {
        log("waiting to get permit to POP")
        emptyQueueLock.acquire()
        lock.acquire()

        log("lock to remove last")
        val toReturn = queue.remove()
        val snapshot = expose()

        log("rm $toReturn -> $snapshot")
        log("unlock")

        lock.release()
        return toReturn
    }

    fun expose(): List<T> {
        return queue.toList()
    }
}
