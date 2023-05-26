namespace Chat.NetworkMessage;

public class UserMessage : Message
{
    private static readonly byte[] Type = { (byte)MessageType.User };
    
    public UserMessage(string text, string username)
    {
        Text = text;
        Username = username;
    }

    public string Text { get; }
    public string Username { get; }

    public override byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        ms.Write(Type);
        ms.Write(BitConverter.GetBytes(Text.Length));
        ms.Write(System.Text.Encoding.UTF8.GetBytes(Text));
        ms.Write(BitConverter.GetBytes(Username.Length));
        ms.Write(System.Text.Encoding.UTF8.GetBytes(Username));
        return ms.ToArray();
    }
}
