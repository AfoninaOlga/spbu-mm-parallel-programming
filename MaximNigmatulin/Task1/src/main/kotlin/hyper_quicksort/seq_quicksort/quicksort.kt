package hyper_quicksort.seq_quicksort

private fun <T> Array<T>.swap(index1: Int, index2: Int) {
    val tmp = this[index1]
    this[index1] = this[index2]
    this[index2] = tmp
}

fun <T : Comparable<T>> partition(a: Array<T>, left: Int, right: Int): Int {
    val pivot = a[(left + right) / 2]
    var i = left
    var j = right

    while (i <= j) {
        while (a[i] < pivot) {
            i++
        }
        while (a[j] > pivot) {
            j--
        }
        if (i >= j) break
        a.swap(i++, j--)
    }

    return j
}


fun <T : Comparable<T>> quicksortBackend(a: Array<T>, l: Int, r: Int) {
    if (l < r) {
        val q = partition(a, l, r)
        quicksortBackend(a, l, q)
        quicksortBackend(a, q + 1, r)
    }
}

fun <T : Comparable<T>> quicksort(a: Array<T>) {
    quicksortBackend(a, 0, a.size - 1)
}

