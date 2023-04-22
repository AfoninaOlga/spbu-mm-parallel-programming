package threadpool

class AggregateException(val aggregated: Exception) : Exception() {
    override val message: String
        get() = "Aggregated: (${aggregated.message})"
}
