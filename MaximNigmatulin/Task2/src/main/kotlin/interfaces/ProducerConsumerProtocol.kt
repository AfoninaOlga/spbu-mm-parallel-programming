package pc.interfaces

interface ProducerConsumerProtocol<T> {
    fun start()
    fun stop()
    fun exposeStorage(): List<T>
}
