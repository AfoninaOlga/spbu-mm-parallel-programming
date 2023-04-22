package threadpool

class AggregateException(private val e: Exception) : Exception() {
    override val message: String
        get() = "Aggregated: (${e.message})"
}
