namespace Chat.NetworkMessage;

public class ConnectMessage : Message
{
    protected bool Equals(ConnectMessage other)
    {
        return NewMember.Equals(other.NewMember);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ConnectMessage)obj);
    }

    public override int GetHashCode()
    {
        return NewMember.GetHashCode();
    }

    private static readonly byte[] Type = { (byte)MessageType.Connect };
    public ConnectMessage(MemberInfo newMember)
    {
        NewMember = newMember;
    }

    public MemberInfo NewMember { get; }
    
    public override byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        ms.Write(Type);
        ms.Write(NewMember.ToBytes());
        return ms.ToArray();
    }
}
