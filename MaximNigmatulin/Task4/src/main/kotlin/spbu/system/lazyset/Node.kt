package spbu.system.lazyset

import java.util.concurrent.locks.ReentrantLock

class Node<T>(var next: Node<T>?, val key: Int, val item: T) {
    private val lock = ReentrantLock()
    var marked = false

    fun lock() {
        lock.lock()
    }

    fun unlock() {
        lock.unlock()
    }
}
