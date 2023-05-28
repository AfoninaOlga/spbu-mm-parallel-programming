namespace Chat.NetworkMessage;

public class DisconnectMessage : Message
{
    protected bool Equals(DisconnectMessage other)
    {
        return DisconnectedMember.Equals(other.DisconnectedMember);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DisconnectMessage)obj);
    }

    public override int GetHashCode()
    {
        return DisconnectedMember.GetHashCode();
    }

    private static readonly byte[] Type = { (byte)MessageType.Disconnect };
    
    public DisconnectMessage(MemberInfo disconnectedMember)
    {
        DisconnectedMember = disconnectedMember;
    }

    public MemberInfo DisconnectedMember { get; }
    
    public override byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        ms.Write(Type);
        ms.Write(DisconnectedMember.ToBytes());
        return ms.ToArray();
    }
}
