package mechanics

import com.google.gson.Gson
import java.net.ConnectException
import java.net.InetSocketAddress
import java.net.ServerSocket
import java.net.Socket
import java.util.*

class ChatAgent(
    val agentName: String,
    hostPort: Int,
    @Volatile var childPort: Int?,
    private val chatReceiverDelegate: ChatReceiverDelegate
) : Thread(), ChatSenderDelegate {

    val isLast: Boolean

    @Volatile
    private var wasConnected = false

    @Volatile
    private var running = false
    private var serverSocket: ServerSocket = ServerSocket()

    init {
        serverSocket.reuseAddress = true
        serverSocket.bind(InetSocketAddress(hostPort))
        isLast = childPort == null
    }

    fun getServerPort(): Int = serverSocket.localPort

    private fun waitForChildMessage() {
        try {
            var parentSocket: Socket?
            while (running) {
                parentSocket = serverSocket.accept()
                val message = decodeMessage(
                    Scanner(parentSocket.getInputStream())
                        .useDelimiter(STREAM_DELIMITER)
                        .next()
                )
                handleMessage(message)
                parentSocket.close()
            }
        } catch (e: Exception) {
            chatReceiverDelegate.onErrorReceived(e)
            shutdown()
        }
    }

    fun propagateMessage(message: Message) {
        childPort?.let { childPort ->
            while (running) {
                try {
                    val childSocket = Socket(LOCALHOST, childPort)
                    val encoded = Gson().toJson(message).toByteArray()
                    childSocket.getOutputStream().write(encoded)
                    childSocket.close()
                    wasConnected = true
                    break
                } catch (e: ConnectException) {
                    if (wasConnected) {
                        chatReceiverDelegate.onErrorReceived(e)
                        shutdown()
                    }
                } catch (e: Exception) {
                    throw e
                }
            }
        }
    }

    override fun run() {
        running = true
        waitForChildMessage()
    }

    private fun decodeMessage(message: String): Message = Gson().fromJson(message, Message::class.java)
    private fun handleMessage(message: Message) {
        when (message.type) {
            MessageType.PASS_THRU ->
                if (message.sender != agentName) {
                    chatReceiverDelegate.onMessageReceived(message)
                    propagateMessage(message)
                }

            MessageType.REGISTER_ME -> {
                chatReceiverDelegate.onAddNewUserToGroup(message)
                if (!isLast) {
                    propagateMessage(message)
                } else {
                    childPort = message.content.toInt()
                }
            }

            MessageType.KILL ->
                if (message.sender != agentName) {
                    chatReceiverDelegate.onUserLeave(message)
                    propagateMessage(message)
                    shutdown()
                }
        }
    }

    fun shutdown() {
        Thread {
            propagateMessage(
                Message(
                    content = EMPTY_MESSAGE,
                    sender = agentName,
                    type = MessageType.KILL
                )
            )
        }.start()
        sleep(KILL_TIMEOUT_MS)
        running = false
        serverSocket.close()
    }

    override fun onMessageSendButtonPress(message: String) {
        propagateMessage(
            Message(
                content = message,
                sender = agentName,
                type = MessageType.PASS_THRU
            )
        )
    }
}
