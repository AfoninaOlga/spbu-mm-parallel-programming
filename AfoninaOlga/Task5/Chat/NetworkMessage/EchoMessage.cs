namespace Chat.NetworkMessage;

public class EchoMessage : Message
{
    private static readonly byte[] Type = { (byte)MessageType.Echo };
    
    public override byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        ms.Write(Type);
        return ms.ToArray();
    }
}
