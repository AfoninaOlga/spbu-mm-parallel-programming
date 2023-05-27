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
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return true;
    }
}
