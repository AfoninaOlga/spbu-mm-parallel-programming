package threadpool

class AggregateException(private val e: Exception) : Exception() {
    override val message: String
        get() = "Aggregated exception message: ${e.message}"

    val aggregated: Exception
        get() = e
}
