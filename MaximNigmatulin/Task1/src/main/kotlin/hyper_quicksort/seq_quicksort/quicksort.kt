package hyper_quicksort.seq_quicksort

fun <T> Array<T>.swap(index1: Int, index2: Int) {
    val tmp = this[index1]
    this[index1] = this[index2]
    this[index2] = tmp
}

fun <T : Comparable<T>> choosePivot(a: Array<T>): T {
    return a[a.size / 2]
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
        if (i <= j) {
            a.swap(i++, j--)
        }
    }
    return i
}

inline fun <reified T : Comparable<T>> partitionWithPivot(a: Array<T>, pivot: T): Triple<Int, Array<T>, Array<T>> {
    var i = 0

    // find first greater then pivot
    while (i < a.size && a[i] < pivot) {
        i++
    }

    var lowLen = i
    var midIndex = i

    // splitting array into [x | x <= pivot], [x | x > pivot]
    while (i < a.size) {
        if (a[i] <= pivot) {
            a.swap(i, midIndex)
            midIndex++
            lowLen++
        }
        i++
    }
    return Triple(
        midIndex,
        a.slice(0 until lowLen).toTypedArray(),
        a.slice(lowLen until a.size).toTypedArray()
    )
}

fun <T : Comparable<T>> quicksortBackend(a: Array<T>, l: Int, r: Int) {
    val q = partition(a, l, r)
    if (l < q - 1) {
        quicksortBackend(a, l, q - 1)
    }
    if (q < r) {
        quicksortBackend(a, q, r)
    }
}

fun <T : Comparable<T>> quicksort(a: Array<T>) {
    if (a.isEmpty()) {
        return
    }
    quicksortBackend(a, 0, a.size - 1)
}
