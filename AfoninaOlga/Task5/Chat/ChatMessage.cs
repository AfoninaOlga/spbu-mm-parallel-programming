namespace Chat;

public record ChatMessage
{
    public record TextMessage(string Sender, string Text) : ChatMessage;
    public record ConnectUserMessage(string User) : ChatMessage;
    public record DisconnectUserMessage(string User) : ChatMessage;

    public string ToText() => this switch
    {
        ConnectUserMessage connectUserMessage => $"User {connectUserMessage.User} connected",
        DisconnectUserMessage disconnectUserMessage => $"User {disconnectUserMessage.User} disconnected",
        TextMessage textMessage => $"{textMessage.Sender}: {textMessage.Text}",
        _ => throw new ArgumentOutOfRangeException()
    };

    private ChatMessage() {}
};
