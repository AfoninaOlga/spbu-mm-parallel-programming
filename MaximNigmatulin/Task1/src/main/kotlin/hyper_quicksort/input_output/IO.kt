package hyper_quicksort.input_output

import java.io.File
import java.io.FileWriter
import java.io.InputStream

fun read(fileName: String): Array<Int> {
    val inputStream: InputStream = File(fileName).inputStream()
    val lineList = mutableListOf<Int>()
    inputStream.bufferedReader().forEachLine { lineList.add(it.toInt()) }
    return lineList.toTypedArray()
}

fun write(fileName: String, data: String) {
    File(fileName).writeText(data)
}

fun generateUnsorted(outFile: String, nNumbers: Int, lowerLimit: Int, upperLimit: Int) {
    val fileWriter = FileWriter(outFile)
    for (i in 0 until nNumbers) {
        val num = (lowerLimit..upperLimit).random()
        fileWriter.write("$num\n")
    }
    fileWriter.close()
}
