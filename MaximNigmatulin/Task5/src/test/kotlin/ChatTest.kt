import mechanics.ChatAgent
import mechanics.Message
import mechanics.MessageType
import org.junit.jupiter.api.Test
import org.junit.jupiter.params.ParameterizedTest
import org.junit.jupiter.params.provider.ValueSource
import java.util.LinkedList
import java.util.concurrent.ConcurrentSkipListSet


class ChatTest {

    @Test
    fun `test instantiation and shutdown`() {
        val receiver = MockReceiver({}, {}, {}, {})
        val agent = ChatAgent("first", 0, null, receiver)
        agent.start()

        agent.shutdown()
    }

    @ParameterizedTest
    @ValueSource(ints = [2, 5, 10, 25, 100])
    fun `test all clients connect and recv messages`(agentCount: Int) {
        val container = ConcurrentSkipListSet<String>()
        val receiver = MockReceiver(
            messageReceived = { container.add(it.sender) },
            {}, {}, {}
        )

        val agents = LinkedList<ChatAgent>()
        var prev: Int? = null
        for (i in 0 until agentCount) {
            val agent = ChatAgent("$i", 0, prev, receiver)
            agent.start()
            Thread {
                if (!agent.isLast) {
                    agent.propagateMessage(
                        Message(
                            content = "${agent.getServerPort()}",
                            sender = "$i",
                            type = MessageType.REGISTER_ME
                        )
                    )
                }
            }.start()

            prev = agent.getServerPort()
            println(prev)
            agents.add(agent)
        }
        Thread.sleep(1000)
        agents.forEach {
            val msg = Message("some data from ${it.agentName}", it.agentName)
            it.propagateMessage(msg)
        }
        Thread.sleep(1000)

        agents.forEach { it.shutdown() }

        for (i in 0 until agentCount) {
            assert(container.contains("$i"))
        }
    }
}
