package hyper_quicksort.app

import hyper_quicksort.seq_quicksort.quicksort
import mpi.MPI

fun main(args: Array<String>) {
    MPI.Init(args)

    val testArray = arrayOf(5, 1, 4, 2, 3)
    quicksort(testArray)
    println(testArray.joinToString(" "))

    val rank = MPI.COMM_WORLD.Rank()
    val size = MPI.COMM_WORLD.Size()
    println("rank=$rank, size=$size")
    MPI.Finalize()
}
