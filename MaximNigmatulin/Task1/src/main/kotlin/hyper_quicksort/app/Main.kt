package hyper_quicksort.app

import hyper_quicksort.input_output.read
import hyper_quicksort.mpi_qucksort.Hypercube
import hyper_quicksort.seq_quicksort.choosePivot
import hyper_quicksort.seq_quicksort.partitionWithPivot
import mpi.MPI
import kotlin.math.pow

fun main(args: Array<String>) {
    MPI.Init(args)

    val input = "./resources/unsorted.txt"
    val output = "./resources/sorted.txt"

    val data: Array<Int> = read(input)
    println(data.joinToString(" "))

    MPI.COMM_WORLD.Barrier()

    val rank = MPI.COMM_WORLD.Rank()
    val size = MPI.COMM_WORLD.Size()

    val isMaster = rank == 0

    var hypercubeDimension = 1
    var validation = 2

    while (validation < size) {
        hypercubeDimension++
        validation = 2.0.pow(hypercubeDimension).toInt()
    }

    if (validation != size) {
        print("Num of dims is not right")
        return
    }

    val hc = Hypercube(hypercubeDimension)

    // --------------- distribute data among the processes ---------------

    val startSize = (data.size / size).toInt()
    var remainder = data.size % size
    val sendcounts = Array(size) { 0 }
    val displacements = Array(size) { 0 }

    for (i in 0 until size) {
        sendcounts[i] = startSize
        if (remainder > 0) {
            sendcounts[i]++
            remainder--
        }

        if (i == 0) {
            displacements[i] = 0
        } else {
            displacements[i] = displacements[i - 1] + sendcounts[i - 1]
        }
    }

    val currentBufferCount = sendcounts[rank]
    val currentBuffer = Array(currentBufferCount) { 0 }
    MPI.COMM_WORLD.Scatterv(
        data,
        0,
        sendcounts.toIntArray(),
        displacements.toIntArray(),
        MPI.OBJECT,
        currentBuffer,
        0,
        currentBufferCount,
        MPI.OBJECT,
        0
    )

    println("rank-buffer mapping:")
    println("$rank : ${currentBuffer.joinToString(" ")}")

    var currentCommunicator = MPI.COMM_WORLD
    for (iteration in 1..1) {
//        for (iteration in 1..hypercubeDimension) {
        val pivotBuffer = IntArray(1) { -1 }
        if (rank == 0) {
            pivotBuffer[0] = choosePivot(currentBuffer)
        }
        println("$rank : ${currentBuffer.joinToString(" ")}")

        currentCommunicator.Bcast(pivotBuffer, 0, 1, MPI.INT, 0)

        println("$rank : pivot = ${pivotBuffer.first()}")
        val pivot = pivotBuffer.first()
        println("$rank : pivot = $pivot")

        // array splitting
        val (midIndex, lowLen, highLen) = partitionWithPivot(currentBuffer, pivot)

        val shouldPassLargerList = hc.shouldPassLargerList(iteration, rank)
        val commLink = hc.getCommLink(iteration, rank)


        // ---------------- exchange sizes ----------------
        var recvLenBuffer = IntArray(1) { -1 }
        val highLenBuffer = IntArray(1) { highLen }
        val lowLenBuffer = IntArray(1) { lowLen }

        if (shouldPassLargerList) {
            MPI.COMM_WORLD.Sendrecv(
                highLenBuffer,
                0,
                1,
                MPI.INT,
                commLink,
                333,
                recvLenBuffer,
                0,
                1,
                MPI.INT,
                commLink,
                333
            )
        } else {
            MPI.COMM_WORLD.Sendrecv(
                lowLenBuffer,
                0,
                1,
                MPI.INT,
                commLink,
                333,
                recvLenBuffer,
                0,
                1,
                MPI.INT,
                commLink,
                333
            )
        }
        val recvLen = recvLenBuffer.first()

        // ---------------- /exchange sizes ----------------

        // init new array
//        val recvBufferLen = if (shouldPassLargerList) highLen else lowLen
        val recvBuffer = IntArray(recvLen) { -1 }
        println("$rank : recv buffer: ${recvBuffer.joinToString(" ")}")

        // send array
        println("$rank : ${currentBuffer.joinToString(" ")}")

        if (shouldPassLargerList) {
            println("(sending hi) $rank : midIndex=$midIndex, hiLen=$highLen, lowLen=$lowLen")
            MPI.COMM_WORLD.Sendrecv(
                currentBuffer.toIntArray(),
                midIndex,
                highLen,
                MPI.INT,
                commLink,
                222,
                recvBuffer,
                0,
                recvLen,
                MPI.INT,
                commLink,
                222
            )
        } else {
            println("(sending lo) $rank : midIndex=$midIndex, hiLen=$highLen, lowLen=$lowLen")
            MPI.COMM_WORLD.Sendrecv(
                currentBuffer.toIntArray(),
                0,
                lowLen,
                MPI.INT,
                commLink,
                222,
                recvBuffer,
                0,
                recvLen,
                MPI.INT,
                commLink,
                222
            )
        }

        val mergedBuffer: Array<Int>
        if (shouldPassLargerList) {
            mergedBuffer = recvBuffer.filterNot { it == -1 }.toTypedArray() + currentBuffer
        } else {
            mergedBuffer = currentBuffer + recvBuffer.filterNot { it == -1 }.toTypedArray()
        }

        println("$rank : merged buffer: ${mergedBuffer.joinToString(" ")}")

        MPI.Finalize()

    }
}