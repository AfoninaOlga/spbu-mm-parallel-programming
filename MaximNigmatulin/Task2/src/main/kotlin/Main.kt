package pc

import pc.impl.ProducerConsumer

fun main(args: Array<String>) {
    require(args.size == 2) { "Wrong number of arguments, 2 expected" }

    args.forEach { println(it) }

    val producersNumber = args[0].toInt()
    val consumerNumber = args[1].toInt()

    val pc = ProducerConsumer(producersNumber, consumerNumber)

    pc.start()
    readln()
    pc.stop()
}
