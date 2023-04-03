package pc.interfaces

interface DataStoreProtocol<T> {
    fun push(obj: T)
    fun pop(): T
    fun expose(): List<T>
}
