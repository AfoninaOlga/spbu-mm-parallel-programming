package utils

import Constants
import java.time.LocalTime

fun parseArgs(args: Array<String>): Int {
    require(args.size == 1) { "Wrong number of arguments, expected 2, got ${args.size}" }
    return args[0].toInt()
}

fun log(msg: String) {
    if (Constants.SHOW_LOGS)
        println("${LocalTime.now()}--[thread ${Thread.currentThread().id}]: $msg")
}
