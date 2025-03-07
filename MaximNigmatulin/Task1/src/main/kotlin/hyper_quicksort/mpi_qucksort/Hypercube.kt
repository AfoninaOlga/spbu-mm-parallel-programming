package hyper_quicksort.mpi_qucksort

import kotlin.math.pow

class Hypercube(private val dimension: Int) {
    fun getNextGroup(iteration: Int, commRank: Int): Int {
        // separate lows and highs to different groups
        val bit = this.dimension - iteration

        val nextGroup: Int = if ((commRank and (1 shl bit)) == 0) {
            0
        } else {
            1
        }

        return nextGroup
    }

    fun shouldPassLargerList(iteration: Int, commRank: Int): Boolean {
        // rule for knowing who is passing larger list
        val bit = this.dimension - iteration
        return commRank and (1 shl bit) == 0
    }

    fun getCommLink(iteration: Int, commRank: Int): Int {
        // depending on iteration choose neighbour with inverted ```bit```
        val bit = this.dimension - iteration
        val mask = 2.0.pow(bit).toInt()
        return commRank xor mask
    }
}
