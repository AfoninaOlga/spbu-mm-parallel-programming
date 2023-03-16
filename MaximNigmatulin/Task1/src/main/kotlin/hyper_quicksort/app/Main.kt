package hyper_quicksort.app

import hyper_quicksort.input_output.generateUnsorted
import hyper_quicksort.input_output.read
import hyper_quicksort.input_output.write
import hyper_quicksort.mpi_qucksort.*
import hyper_quicksort.seq_quicksort.choosePivot
import hyper_quicksort.seq_quicksort.partitionWithPivot
import hyper_quicksort.seq_quicksort.quicksort
import mpi.Group.Translate_ranks
import mpi.Intracomm
import mpi.MPI
import kotlin.math.pow

private fun Int.wrap() = IntArray(1) { this }
private fun IntArray.unwrap() = first()

fun main(args: Array<String>) {
    val (input, output) = args.takeLast(n = 2)
    if (args.size != 5) {
        throw IllegalArgumentException("Wrong number of arguments! Expected 5, received ${args.size}")
    }

    MPI.Init(args)

    generateUnsorted(input, 1_000_000, 1_000_000)

    val rank = MPI.COMM_WORLD.Rank()
    val worldSize = MPI.COMM_WORLD.Size()

    val data: Array<Int> = read(input)

    MPI.COMM_WORLD.Barrier()

    var hypercubeDimension = 1
    var validation = 2

    while (validation < worldSize) {
        hypercubeDimension++
        validation = 2.0.pow(hypercubeDimension).toInt()
    }

    if (validation != worldSize) {
        print("Num of dims is not right")
        MPI.Finalize()
        return
    }

    val hc = Hypercube(hypercubeDimension)

    val startSize = (data.size / worldSize)
    var remainder = data.size % worldSize
    val partsSizes = IntArray(worldSize) { 0 }

    for (partIndex in 0 until worldSize) {
        partsSizes[partIndex] = startSize
        if (remainder > 0) {
            partsSizes[partIndex]++
            remainder--
        }
    }
    val displacements = calculateDisplacements(worldSize, partsSizes)

    val currentBufferCount = partsSizes[rank]
    var currentBuffer = Array(currentBufferCount) { 0 }

    MPI.COMM_WORLD.IntArrayScatterv(
        sendBuffer = data, sendCount = partsSizes,
        displs = displacements,
        recvBuffer = currentBuffer
    )

    var currentCommunicator = MPI.COMM_WORLD
    for (iteration in 1..hypercubeDimension) {
        val pivot = broadcastPivot(worldSize, currentCommunicator, currentBuffer)

        if (pivot == -1) {
            // buffer is empty, nothing to do
            if (currentBuffer.isNotEmpty()) {
                throw RuntimeException("$rank has -1 for pivot!")
            }
            continue
        }

        val (midIndex, lowArray, highArray) = partitionWithPivot(currentBuffer, pivot)

        val shouldPassLargerList = hc.shouldPassLargerList(iteration, rank)
        val commLink = hc.getCommLink(iteration, rank)

        val recvLenWrapped = IntArray(1)

        // sending len of part (low | hi) that will be sent after
        MPI.COMM_WORLD.intArraySendrecv(
            sendBuffer = if (shouldPassLargerList) highArray.size.wrap() else lowArray.size.wrap(),
            source = commLink,
            recvBuffer = recvLenWrapped,
            dest = commLink
        )

        val recvBuffer = IntArray(recvLenWrapped.unwrap())

        // exchange low high
        MPI.COMM_WORLD.intArraySendrecv(
            sendBuffer = currentBuffer.toIntArray(),
            sendOffset = if (shouldPassLargerList) midIndex else 0,
            sendCount = if (shouldPassLargerList) highArray.size else lowArray.size,
            source = commLink,
            recvBuffer = recvBuffer,
            dest = commLink
        )

        currentBuffer = if (shouldPassLargerList) {
            recvBuffer.toTypedArray() + lowArray
        } else {
            highArray + recvBuffer.toTypedArray()
        }

        val nextGroup = hc.getNextGroup(iteration, rank)
        val nextCommunicator = currentCommunicator.Split(nextGroup, rank)
        currentCommunicator = nextCommunicator
    }

    quicksort(currentBuffer)

    val mergedArray = mergeParts(
        partsCount = worldSize,
        resultSize = data.size,
        currentPart = currentBuffer
    )

    if (rank == 0) {
        write(output, mergedArray.joinToString(" "))
    }

    MPI.Finalize()
}

private fun mergeParts(
    partsCount: Int,
    resultSize: Int,
    currentPart: Array<Int>
): IntArray {
    val partsSizes = IntArray(partsCount)
    val currentBufferCountBuffer = IntArray(1) { currentPart.size }
    MPI.COMM_WORLD.intArrayGather(
        sendBuffer = currentBufferCountBuffer, sendCount = 1,
        recvBuffer = partsSizes, recvCount = 1
    )

    val displs = calculateDisplacements(partsCount, partsSizes)
    if (MPI.COMM_WORLD.Rank() == 0) {
        calculateDisplacements(partsCount, partsSizes)
    }
    val mergedArray = IntArray(resultSize)
    MPI.COMM_WORLD.intArrayGatherv(
        sendBuffer = currentPart.toIntArray(),
        recvBuffer = mergedArray, recvCount = partsSizes,
        displs = displs
    )
    return mergedArray
}

private fun calculateDisplacements(partsCount: Int, partsSizes: IntArray): IntArray {
    val displs = IntArray(partsCount)
    displs[0] = 0
    for (i in 1 until partsCount) {
        displs[i] = displs[i - 1] + partsSizes[i - 1]
    }

    return displs
}

private fun translateRanks(size: Int, currentCommunicator: Intracomm): List<Int> {
    return Translate_ranks(
        MPI.COMM_WORLD.group,
        IntArray(size) { it },
        currentCommunicator.group
    ).filterNot { it == -1 }
}

private fun broadcastPivot(
    size: Int,
    currentCommunicator: Intracomm,
    currentBuffer: Array<Int>,
): Int {
    val pivotWrapped = (-1).wrap()
    val translated = translateRanks(size, currentCommunicator)

    // search for process with non-empty buffer
    for (groupCommunicator in translated) {
        if (currentCommunicator.Rank() != groupCommunicator) {
            continue
        }
        if (currentBuffer.isEmpty()) {
            continue
        }
        pivotWrapped[0] = choosePivot(currentBuffer)
        break
    }

    currentCommunicator.Bcast(pivotWrapped, 0, 1, MPI.INT, 0)
    return pivotWrapped.unwrap()
}
