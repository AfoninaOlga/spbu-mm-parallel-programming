package mechanics

interface ChatSenderDelegate {
    fun onMessageSendButtonPress(message: String)
}

interface ChatReceiverDelegate {
    fun onMessageReceived(message: Message)
    fun onErrorReceived(e: Exception)
    fun onAddNewUserToGroup(message: Message)
    fun onUserLeave(message: Message)
}
