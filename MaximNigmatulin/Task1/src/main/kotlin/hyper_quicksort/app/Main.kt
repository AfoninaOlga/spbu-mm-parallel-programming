package hyper_quicksort.app

import hyper_quicksort.input_output.read
import hyper_quicksort.mpi_qucksort.Hypercube
import hyper_quicksort.seq_quicksort.choosePivot
import hyper_quicksort.seq_quicksort.partitionWithPivot
import mpi.Group.Translate_ranks
import mpi.MPI
import kotlin.math.pow

fun main(args: Array<String>) {
    MPI.Init(args)

    val input = "./resources/unsorted.txt"
    val output = "./resources/sorted.txt"

    if (MPI.COMM_WORLD.Rank() == 0) {
        val testArray = read(input)
        println(testArray.joinToString(" "))
    }

    val data: Array<Int> = read(input)
    println(data.joinToString(" "))

    MPI.COMM_WORLD.Barrier()

    val rank = MPI.COMM_WORLD.Rank()
    val size = MPI.COMM_WORLD.Size()

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

    var currentBufferCount = sendcounts[rank]
    var currentBuffer = Array(currentBufferCount) { 0 }
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

    var currentCommunicator = MPI.COMM_WORLD
    for (iteration in 1..hypercubeDimension) {

        // ---------------- choose pivot ----------------
        val pivotBuffer = IntArray(1) { -1 }
        val translated =
            Translate_ranks(
                MPI.COMM_WORLD.group,
                IntArray(size) { it },
                currentCommunicator.group
            ).filterNot { it == -1 }

        for (groupProcessor in translated) {
            if (currentCommunicator.Rank() != groupProcessor) {
                continue
            }
            if (currentBuffer.isEmpty()) {
                continue
            }
            pivotBuffer[0] = choosePivot(currentBuffer)
            break
        }
        MPI.COMM_WORLD.Barrier()

        // ---------------- /choose pivot ----------------

        currentCommunicator.Bcast(pivotBuffer, 0, 1, MPI.INT, 0)
        if (pivotBuffer.first() == -1) {
            // buffer is empty, nothing to do
            if (currentBuffer.isNotEmpty()) {
                throw RuntimeException("$rank has -1 for pivot!")
            }
            continue
        }
        val pivot = pivotBuffer.first()

        // array splitting
        val (midIndex, lowArray, highArray) = partitionWithPivot(currentBuffer, pivot)
        val lowLen = lowArray.size
        val highLen = highArray.size

        val shouldPassLargerList = hc.shouldPassLargerList(iteration, rank)
        val commLink = hc.getCommLink(iteration, rank)

        // ---------------- exchange sizes ----------------
        val recvLenBuffer = IntArray(1) { -1 }
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
        val recvBuffer = IntArray(recvLen) { -1 }

        // send array
        if (shouldPassLargerList) {
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

        val mergedBuffer: Array<Int> = if (shouldPassLargerList) {
            recvBuffer.toTypedArray() + lowArray
        } else {
            highArray + recvBuffer.toTypedArray()
        }

        currentBuffer = mergedBuffer
        currentBufferCount = recvLen

        val nextGroup = hc.getNextGroup(iteration, rank)
        val nextCommunicator = currentCommunicator.Split(nextGroup, rank)
        currentCommunicator = nextCommunicator
    }

    currentBuffer.sort()
    println("$rank : final (sorted) buffer: ${currentBuffer.joinToString(" ")}")

    // ---------------- gather values ----------------
    MPI.COMM_WORLD.Barrier()

    val gatheredSizes = IntArray(size) { -1 }
    val currentBufferCountBuffer = IntArray(1) { currentBufferCount + 1 }
    println("$rank : ${currentBufferCountBuffer.joinToString(" ")}")
    MPI.COMM_WORLD.Gather(currentBufferCountBuffer, 0, 1, MPI.INT, gatheredSizes, 0, 1, MPI.INT, 0)
    if (rank == 0)
        println(gatheredSizes.joinToString(" "))

    MPI.Finalize()
}
