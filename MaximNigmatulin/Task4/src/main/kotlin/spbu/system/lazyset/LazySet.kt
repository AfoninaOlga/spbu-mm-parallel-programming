package spbu.system.lazyset

import spbu.models.StudentRecord
import spbu.system.ExamSystemInterface

class LazySet : ExamSystemInterface {
    private val head = Node(
        next = null,
        key = -1,
        item = StudentRecord(-1, -1)
    )

    override fun add(record: StudentRecord): Boolean {
        val key = record.hashCode()
        while (true) {
            var pred = head
            var curr = pred.next

            while (curr != null && curr.key < key) {
                pred = curr
                curr = curr.next
            }
            pred.lock()
            try {
                curr?.lock()
                try {
                    if (validate(pred, curr)) {
                        if (curr?.key == key) {
                            return false
                        }
                        val insert = Node(curr, key, record)
                        pred.next = insert
                        return true
                    }
                } finally {
                    curr?.unlock()
                }
            } finally {
                pred.unlock()
            }
        }
    }

    override fun remove(record: StudentRecord): Boolean {
        val key = record.hashCode()
        while (true) {
            var pred = head
            var curr = pred.next

            while (curr != null && curr.key < key) {
                pred = curr
                curr = curr.next
            }
            pred.lock()
            try {
                curr?.lock()
                try {
                    if (validate(pred, curr)) {
                        if (curr?.key != key) {
                            return false
                        }
                        curr.marked = true
                        pred.next = curr.next
                        return true
                    }
                } finally {
                    curr?.unlock()
                }
            } finally {
                pred.unlock()
            }
        }
    }

    override fun contains(record: StudentRecord): Boolean {
        val key = record.hashCode()
        var curr: Node<StudentRecord>? = head
        while (curr != null && curr.key < key) {
            curr = curr.next
        }
        return curr != null && !curr.marked && curr.key == key
    }

    override fun count(): Int {
        var curr: Node<StudentRecord>? = head
        var count = 0
        while (curr != null) {
            curr = curr.next
            curr?.let {
                if (!curr.marked)
                    count += 1
            }
        }
        return count
    }

    private fun <StudentRecord> validate(pred: Node<StudentRecord>, curr: Node<StudentRecord>?): Boolean {
        var node: Node<spbu.models.StudentRecord>? = this.head
        while (node != null && node.key <= pred.key) {
            if (node == pred) {
                return pred.next == curr
            }
            node = node.next
        }
        return false
    }
}
