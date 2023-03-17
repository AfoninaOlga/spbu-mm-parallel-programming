package hyper_quicksort.input_output

import java.io.File
import java.io.FileWriter

fun read(fileName: String): Array<Int> {
    return File(fileName)
        .readText()
        .split(" ")
        .map { it.toInt() }
        .toTypedArray()
}

fun write(fileName: String, data: String) {
    File(fileName).writeText(data)
}

fun generateUnsorted(outFile: String, nNumbers: Int, lowerLimit: Int, upperLimit: Int) {
    val fileWriter = FileWriter(outFile)
    for (i in 1 until nNumbers) {
        val num = (lowerLimit..upperLimit).random()
        fileWriter.write("$num ")
    }
    val num = (lowerLimit..upperLimit).random()
    fileWriter.write("$num")
    fileWriter.close()
}
