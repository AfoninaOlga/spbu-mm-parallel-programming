package hyper_quicksort.mpi_qucksort

import mpi.Intracomm
import mpi.MPI
import mpi.Status

fun Intracomm.intArraySendrecv(
    sendBuffer: IntArray,
    sendOffset: Int = 0,
    sendCount: Int = sendBuffer.size,
    source: Int,
    recvBuffer: IntArray,
    recvOffset: Int = 0,
    recvCount: Int = recvBuffer.size,
    dest: Int,
    sendTag: Int = 78
): Status = Sendrecv(
    sendBuffer, sendOffset, sendCount, MPI.INT, source, sendTag,
    recvBuffer, recvOffset, recvCount, MPI.INT, dest, sendTag
)

fun Intracomm.intArrayGather(
    sendBuffer: IntArray,
    sendOffset: Int = 0,
    sendCount: Int = sendBuffer.size,
    recvBuffer: IntArray,
    recvOffset: Int = 0,
    recvCount: Int = recvBuffer.size,
    root: Int = 0
) = Gather(
    sendBuffer, sendOffset, sendCount, MPI.INT,
    recvBuffer, recvOffset, recvCount, MPI.INT, root
)

fun Intracomm.intArrayGatherv(
    sendBuffer: IntArray,
    sendOffset: Int = 0,
    sendCount: Int = sendBuffer.size,
    recvBuffer: IntArray,
    recvOffset: Int = 0,
    recvCount: IntArray,
    displs: IntArray,
    root: Int = 0
) = Gatherv(sendBuffer, sendOffset, sendCount, MPI.INT, recvBuffer, recvOffset, recvCount, displs, MPI.INT, root)

fun Intracomm.IntArrayScatterv(
    sendBuffer: Array<Int>,
    sendOffset: Int = 0,
    sendCount: IntArray,
    displs: IntArray,
    recvBuffer: Array<Int>,
    recvOffset: Int = 0,
    recvCount: Int = recvBuffer.size,
    root: Int = 0
) = Scatterv(
    sendBuffer, sendOffset, sendCount, displs, MPI.OBJECT,
    recvBuffer, recvOffset, recvCount, MPI.OBJECT, root
)
