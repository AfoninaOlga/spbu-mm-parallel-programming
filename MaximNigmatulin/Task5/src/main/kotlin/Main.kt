import javafx.application.Application
import javafx.application.Platform
import javafx.event.EventHandler
import javafx.scene.Scene
import javafx.scene.control.Button
import javafx.scene.control.Label
import javafx.scene.control.ScrollPane
import javafx.scene.control.TextField
import javafx.scene.layout.AnchorPane
import javafx.scene.layout.Region
import javafx.scene.layout.VBox
import javafx.stage.Stage
import mechanics.*

class ChatApp : Application(), ChatReceiverDelegate {

    private var scrollPane: ScrollPane? = null
    private var dialogContainer: VBox? = null
    private var userInput: TextField? = null
    private var sendButton: Button? = null
    private var scene: Scene? = null

    private var chatSenderDelegate: ChatSenderDelegate? = null

    override fun start(stage: Stage) {
        scrollPane = ScrollPane()
        dialogContainer = VBox()
        scrollPane!!.content = dialogContainer

        userInput = TextField()
        sendButton = Button("Send")

        require(parameters.unnamed.size == 2 || parameters.unnamed.size == 3) {
            throw IllegalArgumentException("Wrong argument count")
        }
        val name = parameters.unnamed[0]
        val selfPort = parameters.unnamed[1].toInt()
        val childPort = if (parameters.unnamed.size == 3) {
            parameters.unnamed[2].toInt()
        } else null

        println("$name is running on port $selfPort and expects child to run on $childPort")
        val agent = ChatAgent(name, selfPort, childPort, this)
        agent.start()
        Thread {
            if (!agent.isLast) {
                agent.propagateMessage(
                    Message(
                        content = "$selfPort",
                        sender = name,
                        type = MessageType.REGISTER_ME
                    )
                )
            }
        }.start()
        chatSenderDelegate = agent

        layout("P2P Chat: $name", stage)
        createListeners()

        stage.scene = scene

        stage.setOnCloseRequest {
            agent.shutdown()
            println("agent $selfPort died")
        }
        stage.show()
    }

    private fun sendUserInput() {
        val text = userInput!!.text
        if (text == "") return
        chatSenderDelegate?.onMessageSendButtonPress(text)
        dialogContainer!!.children.add(getDialogLabel("(me): $text"))
        userInput!!.clear()
    }

    private fun createListeners() {
        sendButton!!.onMouseClicked = EventHandler {
            sendUserInput()
        }

        userInput!!.onAction = EventHandler {
            sendUserInput()
        }

        dialogContainer!!.heightProperty().addListener { _, _, _ ->
            scrollPane!!.vvalue = 1.0
        }
    }

    private fun layout(title: String, stage: Stage) {
        val mainLayout = AnchorPane()
        mainLayout.children.addAll(scrollPane, userInput, sendButton)

        scene = Scene(mainLayout)

        stage.title = title
        stage.isResizable = false;
        stage.minHeight = 600.0;
        stage.minWidth = 400.0;

        mainLayout.setPrefSize(400.0, 600.0);

        scrollPane!!.setPrefSize(385.0, 535.0);
        scrollPane!!.hbarPolicy = ScrollPane.ScrollBarPolicy.NEVER;
        scrollPane!!.vbarPolicy = ScrollPane.ScrollBarPolicy.ALWAYS;

        scrollPane!!.vvalue = 1.0;
        scrollPane!!.isFitToWidth = true;

        dialogContainer!!.prefHeight = Region.USE_COMPUTED_SIZE;

        userInput!!.prefWidth = 325.0;

        sendButton!!.prefWidth = 55.0;

        AnchorPane.setTopAnchor(scrollPane, 1.0);

        AnchorPane.setBottomAnchor(sendButton, 1.0);
        AnchorPane.setRightAnchor(sendButton, 1.0);

        AnchorPane.setLeftAnchor(userInput, 1.0);
        AnchorPane.setBottomAnchor(userInput, 1.0);
    }

    override fun onMessageReceived(message: Message) {
        addDialogLabel(message)
    }

    override fun onErrorReceived(e: Exception) {
        addErrorLabel(e)
    }

    override fun onAddNewUserToGroup(message: Message) {
        addUserLabel(message)
    }

    override fun onUserLeave(message: Message) {
        addUserLeftLabel(message)
        userInput?.disableProperty()?.set(true)
        sendButton?.disableProperty()?.set(true)
    }

    private fun addDialogLabel(message: Message) {
        Platform.runLater {
            dialogContainer!!.children.add(getDialogLabel("(${message.sender}): ${message.content}"))
        }
    }

    private fun addErrorLabel(e: Exception) {
        Platform.runLater {
            dialogContainer!!.children.add(getDialogLabel("ERROR! ${e.stackTraceToString()}"))
        }
    }

    private fun addUserLabel(message: Message) {
        Platform.runLater {
            dialogContainer!!.children.add(getDialogLabel("${message.sender} joined"))
        }
    }

    private fun addUserLeftLabel(message: Message) {
        Platform.runLater {
            dialogContainer!!.children.add(getDialogLabel("${message.sender} left. Chat is now closed"))
        }
    }

}

private fun getDialogLabel(text: String): Label {
    val textToAdd = Label(text)
    textToAdd.isWrapText = true
    return textToAdd
}

fun main(args: Array<String>) {
    print(args)
    Application.launch(ChatApp::class.java)
}