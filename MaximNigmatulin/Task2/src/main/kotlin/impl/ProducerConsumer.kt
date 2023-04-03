package pc.impl

import pc.interfaces.Consumer
import pc.interfaces.Producer
import pc.interfaces.ProducerConsumerProtocol
import pc.interfaces.Waiter
import pc.threading.CancellableThread

class ProducerConsumer(
    producers: List<Producer<Int>>,
    consumers: List<Consumer<Int>>,
    waiter: Waiter = Waiter { Thread.sleep((500L..1000L).random()) }
) :
    ProducerConsumerProtocol<Int> {

    private val dq = DataStore<Int>()
    private val producers: List<CancellableThread>
    private val consumers: List<CancellableThread>

    init {
        this.producers = producers.map { producer ->
            CancellableThread {
                dq.push(producer.produce())
                waiter.sleep()
            }
        }

        this.consumers = consumers.map { consumer ->
            CancellableThread {
                consumer.consume(dq.pop())
                waiter.sleep()
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

    override fun isStopped(): Boolean {
        return (consumers + producers).map { !it.isAlive }.all { it }
    }
}
