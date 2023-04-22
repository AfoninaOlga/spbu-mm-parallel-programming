package extensions

import threadpool.INamedRunnable
import java.util.concurrent.BlockingQueue

fun BlockingQueue<INamedRunnable>.stringify(): String {
    var s = ""

    synchronized(this) {
        s += "<"
        for (item in this) {
            s += "["
            s += item.name
            s += "]"
        }
        s += ">"
    }

    return s
}
