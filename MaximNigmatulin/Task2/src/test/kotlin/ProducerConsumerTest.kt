import org.junit.jupiter.api.RepeatedTest
import pc.impl.ProducerConsumer


class ProducerConsumerTest {

    @RepeatedTest(5)
    fun `test threads not alive after immediate stop`() {
        val pc = ProducerConsumer(10, 10)

        pc.start()
        pc.stop()

        assert(pc.isStopped())
    }

    @RepeatedTest(5)
    fun `test threads not alive after stop`() {
        val pc = ProducerConsumer(10, 10)

        pc.start()
        Thread.sleep(2000)
        pc.stop()

        assert(pc.isStopped())
    }
}
