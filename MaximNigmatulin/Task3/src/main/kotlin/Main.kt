import threadpool.ThreadPoolTask
import threadpool.ThreadPool
import threadpool.WorkStrategy
import utils.parseArgs


fun main(args: Array<String>) {
    val nThreads = parseArgs(args)
    val pool = ThreadPool(nThreads, WorkStrategy.SHARING)

    val root = ThreadPoolTask("root", pool) {
        Thread.sleep(1000)
        throw RuntimeException("Root error!")
        2
    }

    val onceContinuedRoot = root.continueWith {
        Thread.sleep(1000)
        2
    }

    val twiceContinuedOne = root.continueWith {
        Thread.sleep(1000)
        it * 2
    }.continueWith {
        Thread.sleep(1000)
        it * 2
    }

    val thriceContinuedTwo = twiceContinuedOne.continueWith {
        Thread.sleep(1000)
        it * 2
    }

    pool.enqueue(root)

    pool.start()
    readln()

    pool.use {
        thriceContinuedTwo.result()
    }
}
