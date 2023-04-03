package pc

import pc.impl.ProducerConsumer
import pc.interfaces.Producer
import pc.interfaces.Consumer
import java.time.LocalDate

fun parseArgs(args: Array<String>): Pair<Int, Int> {
    require(args.size == 2) { "Wrong number of arguments, expected 2, got ${args.size}" }
    return Pair(args[0].toInt(), args[1].toInt())
}

fun log(msg: String) {
    println("${LocalDate.now()} | ${Thread.currentThread().id}: $msg")
}

fun main(args: Array<String>) {
    val (producersNumber, consumersNumber) = parseArgs(args)

    val pc = ProducerConsumer(
        producers=(0 until producersNumber).map { Producer { Thread.currentThread().id.toInt() } },
        consumers=(0 until consumersNumber).map { Consumer { } },
        log=::log
    )

    pc.start()
    readln()
    pc.stop()
}
