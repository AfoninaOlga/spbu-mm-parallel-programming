package threadpool

interface INamedRunnable : Runnable {
    val name: String
}