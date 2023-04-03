import org.junit.jupiter.api.RepeatedTest
import pc.impl.ProducerConsumer
import pc.interfaces.Consumer
import pc.interfaces.Producer
import pc.interfaces.Waiter
import java.util.concurrent.LinkedBlockingQueue
import kotlin.test.assertEquals


class ProducerConsumerTest {

    companion object {
        class TestDataProvider {
            companion object {
                const val END = -1
                val START_DATA = listOf(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12)
            }

            private val data = LinkedBlockingQueue(START_DATA)

            fun provide(): Int {
                synchronized(this) {
                    if (!data.isEmpty())
                        return data.remove()
                    return END
                }
            }
        }
    }

    @RepeatedTest(5)
    fun `test threads not alive after immediate stop`() {
        val pc = ProducerConsumer(
            (0 until 10).map { Producer { it } },
            (0 until 10).map { Consumer { } },
        ) { Thread.sleep(1) }

        pc.start()
        pc.stop()

        assert(pc.isStopped())
    }

    @RepeatedTest(5)
    fun `test threads alive after start and not alive after stop`() {
        val pc = ProducerConsumer(
            (0 until 10).map { Producer { it } },
            (0 until 10).map { Consumer { } },
        ) { Thread.sleep(1) }

        pc.start()
        Thread.sleep(1000)
        assert(!pc.isStopped())

        pc.stop()

        assert(pc.isStopped())
    }

    @RepeatedTest(5)
    fun `test 1 consumer 1 producer consumed all`() {
        val tdp = TestDataProvider()
        val results = LinkedBlockingQueue<Int>()

        val pc = ProducerConsumer(
            (0 until 1).map { Producer { tdp.provide() } },
            (0 until 1).map { Consumer { if (it != TestDataProvider.END) results.add(it) } },
            Waiter { Thread.sleep(1) }
        )

        pc.start()
        Thread.sleep(100)
        pc.stop()

        assertEquals(TestDataProvider.START_DATA.toSet(), results.toSet())
    }

    @RepeatedTest(5)
    fun `test N consumers 1 producer consumed all`() {
        val tdp = TestDataProvider()
        val results = LinkedBlockingQueue<Int>()

        val pc = ProducerConsumer(
            (0 until 1).map { Producer { tdp.provide() } },
            (0 until 1).map { Consumer { if (it != TestDataProvider.END) results.add(it) } },
            Waiter { Thread.sleep(1) }
        )

        pc.start()
        Thread.sleep(100)
        pc.stop()

        assertEquals(TestDataProvider.START_DATA.toSet(), results.toSet())
    }

    @RepeatedTest(5)
    fun `test 1 consumer N producers consumed all`() {
        val tdp = TestDataProvider()
        val results = LinkedBlockingQueue<Int>()

        val pc = ProducerConsumer(
            (0 until 1).map { Producer { tdp.provide() } },
            (0 until 1).map { Consumer { if (it != TestDataProvider.END) results.add(it) } },
            Waiter { Thread.sleep(1) }
        )

        pc.start()
        Thread.sleep(100)
        pc.stop()

        assertEquals(TestDataProvider.START_DATA.toSet(), results.toSet())
    }

    @RepeatedTest(5)
    fun `test N consumers N producers consumed all`() {
        val tdp = TestDataProvider()
        val results = LinkedBlockingQueue<Int>()

        val pc = ProducerConsumer(
            (0 until 1).map { Producer { tdp.provide() } },
            (0 until 1).map { Consumer { if (it != TestDataProvider.END) results.add(it) } },
            Waiter { Thread.sleep(1) }
        )

        pc.start()
        Thread.sleep(100)
        pc.stop()

        assertEquals(TestDataProvider.START_DATA.toSet(), results.toSet())
    }
}
