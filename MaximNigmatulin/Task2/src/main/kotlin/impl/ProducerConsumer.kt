package pc.impl

import pc.interfaces.ProducerConsumerProtocol

class ProducerConsumer(nProducers: Int, nConsumers: Int) :
    ProducerConsumerProtocol<Int> {

    private val dq = DataStore<Int>()
    private val producers: List<CancellableThread>
    private val consumers: List<CancellableThread>

    init {
        producers = (0 until nProducers).map { threadIndex ->
            CancellableThread {
                dq.push(threadIndex)
                Thread.sleep((500L..1000L).random())
            }
        }

        consumers = (0 until nConsumers).map {
            CancellableThread {
                dq.pop()
                Thread.sleep((500L..1000L).random())
            }
        }
    }

    override fun start() {
        (consumers + producers).forEach { it.start() }
    }

    override fun stop() {
        (consumers + producers).forEach { it.cancel() }
        (consumers + producers).forEach { it.join() }
    }

    override fun exposeStorage(): List<Int> {
        return dq.expose()
    }

    fun isStopped(): Boolean {
        return (consumers + producers).map { !it.isAlive }.all { it }
    }
}
