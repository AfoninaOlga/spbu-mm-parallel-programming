package hyper_quicksort.app

import hyper_quicksort.input_output.generateUnsorted
import hyper_quicksort.input_output.read
import hyper_quicksort.input_output.write
import hyper_quicksort.seq_quicksort.quicksort
import mpi.MPI

fun main(args: Array<String>) {
    MPI.Init(args)

    val input = "./resources/unsorted.txt"
    val output = "./resources/sorted.txt"

    if (MPI.COMM_WORLD.Rank() == 0) {
        generateUnsorted(input, 10, 100)
        val testArray = read(input)
        quicksort(testArray)
        println(testArray.joinToString(" "))
        write(output, testArray.joinToString(" "))
    }
    MPI.Finalize()
}
