package spbu.system.finegrained

import spbu.models.StudentRecord
import spbu.system.ExamSystemInterface

class FineGrainedSet : ExamSystemInterface {
    private val head = Node(
        next = null,
        key = -1,
        item = StudentRecord(-1, -1)
    )

    override fun add(record: StudentRecord): Boolean {
        val key = record.hashCode()
        head.lock()
        var pred = head
        try {
            if (pred.next == null) {
                pred.next = Node(null, key, record)
                return true
            }
            var curr = pred.next
            curr?.lock()
            try {
                while (curr != null && curr.key < key) {
                    pred.unlock()
                    pred = curr
                    curr = curr.next
                    curr?.lock()
                }
                if (curr != null && curr.key == key) {
                    return false
                }
                val insert = Node(curr, key, record)
                pred.next = insert
                return true
            } finally {
                curr?.unlock()
            }
        } finally {
            pred.unlock()
        }
    }

    override fun remove(record: StudentRecord): Boolean {
        val key = record.hashCode()
        head.lock()
        var pred = head
        try {
            if (pred.next == null) {
                pred.next = Node(null, key, record)
                return true
            }
            var curr = pred.next
            curr?.lock()
            try {
                while (curr != null && curr.key < key) {
                    pred.unlock()
                    pred = curr
                    curr = curr.next
                    curr?.lock()
                }
                if (curr != null && curr.key == key) {
                    pred.next = curr.next
                    return true
                }
                return false
            } finally {
                curr?.unlock()
            }
        } finally {
            pred.unlock()
        }
    }

    override fun contains(record: StudentRecord): Boolean {
        val key = record.hashCode()
        head.lock()
        var pred = head
        try {
            if (pred.next == null) {
                return false
            }
            var curr = pred.next
            curr?.lock()
            try {
                while (curr != null && curr.key < key) {
                    pred.unlock()
                    pred = curr
                    curr = curr.next
                    curr?.lock()
                }
                if (curr != null && curr.key == key) {
                    return true
                }
                return false
            } finally {
                curr?.unlock()
            }
        } finally {
            pred.unlock()
        }
    }

    override fun count(): Int {
        try {
            head.lock()
            var curr = head.next
            var count = 0
            while (curr != null) {
                curr.lock()
                curr = curr.next
                count += 1
            }
            return count
        } finally {
            var curr = head.next
            head.unlock()
            while (curr != null) {
                curr = curr.next
                curr?.unlock()
            }
        }
    }
}
