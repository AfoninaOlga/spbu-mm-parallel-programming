import threadpool.ThreadPoolTask
import threadpool.ThreadPool
import utils.parseArgs


fun main(args: Array<String>) {
    val nThreads = parseArgs(args)
    val pool = ThreadPool(nThreads)

    val root = ThreadPoolTask("root", pool) {
        Thread.sleep(1000)
        2
    }

    val twiceContinuedOne = root.continueWith {
        Thread.sleep(1000)
        it * 2
    }.continueWith {
        Thread.sleep(1000)
        throw RuntimeException("TESTTEST")
        it * 2
    }

    val thriceContinuedOne = twiceContinuedOne.continueWith {
        Thread.sleep(1000)
        it * 2
    }

    val thriceContinuedTwo = twiceContinuedOne.continueWith {
        Thread.sleep(1000)
        it * 2
    }

    pool.enqueue(root)

//    (0..10).forEach {
//        pool.enqueue(ThreadPoolTask(it.toString(), pool) {
//            Thread.sleep(1000)
//        })
//    }

    pool.start()
    readln()
    pool.close()
}
