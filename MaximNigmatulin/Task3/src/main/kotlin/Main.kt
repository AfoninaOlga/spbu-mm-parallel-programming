import threadpool.ThreadPoolTask
import threadpool.ThreadPool
import threadpool.WorkStrategy
import utils.parseArgs


fun main(args: Array<String>) {
    val nThreads = parseArgs(args)
    val pool = ThreadPool(nThreads, WorkStrategy.SHARING)

    val root = ThreadPoolTask("root", pool) {
        Thread.sleep(1000)
        2
    }

    val onceContinuedRoot = root.continueWith {
        Thread.sleep(1000)
        2
    }

    val thriceContinuedOne = root.continueWith {
        Thread.sleep(1000)
        it * 2
    }.continueWith {
        Thread.sleep(1000)
        it * 2
    }

    pool.enqueue(root)

    (0..10).forEach {
        pool.enqueue(
            ThreadPoolTask(it.toString(), pool) {
                Thread.sleep(1000)
                2
            }
        )
    }

    pool.start()
    readln()
    pool.close()
}
