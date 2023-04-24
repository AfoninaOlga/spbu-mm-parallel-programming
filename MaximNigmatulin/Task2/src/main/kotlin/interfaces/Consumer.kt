package pc.interfaces

fun interface Consumer<T> {
    fun consume(item: T?)
}
