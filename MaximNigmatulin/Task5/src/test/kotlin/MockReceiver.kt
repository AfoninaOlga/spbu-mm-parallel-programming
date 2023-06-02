import mechanics.ChatReceiverDelegate
import mechanics.Message

class MockReceiver(
    val messageReceived: (Message) -> Unit,
    val errorReceived: (Exception) -> Unit,
    val newUserAdded: (Message) -> Unit,
    val userLeft: (Message) -> Unit
) : ChatReceiverDelegate {
    override fun onMessageReceived(message: Message): Unit = messageReceived(message)
    override fun onErrorReceived(e: Exception): Unit = errorReceived(e)
    override fun onAddNewUserToGroup(message: Message): Unit = newUserAdded(message)
    override fun onUserLeave(message: Message): Unit = userLeft(message)
}