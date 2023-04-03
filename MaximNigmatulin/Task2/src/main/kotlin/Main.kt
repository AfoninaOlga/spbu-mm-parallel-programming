package pc

import pc.impl.ProducerConsumer
import pc.interfaces.Producer
import pc.interfaces.Consumer

fun parseArgs(args: Array<String>): Pair<Int, Int> {
    require(args.size == 2) { "Wrong number of arguments, expected 2, got ${args.size}" }
    return Pair(args[0].toInt(), args[1].toInt())
}

fun main(args: Array<String>) {
    val (producersNumber, consumersNumber) = parseArgs(args)

    val pc = ProducerConsumer(
        (0 until producersNumber).map { Producer { it } },
        (0 until consumersNumber).map { Consumer { } },
    )

    pc.start()
    readln()
    pc.stop()
}
