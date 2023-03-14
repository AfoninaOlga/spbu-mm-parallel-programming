package hyper_quicksort.app

import mpi.MPI

fun main(args: Array<String>) {
    MPI.Init(args)
    val rank = MPI.COMM_WORLD.Rank()
    val size = MPI.COMM_WORLD.Size()
    println("rank=$rank, size=$size")
    MPI.Finalize()
}
