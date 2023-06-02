package mechanics

class Message(
    val content: String,
    val sender: String,
    val type: MessageType = MessageType.PASS_THRU
) {
    override fun toString() = "<<<($sender) $content: of type $type>>>"
}

enum class MessageType {
    PASS_THRU,
    REGISTER_ME,
    KILL
}
