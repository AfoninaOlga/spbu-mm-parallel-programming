namespace Chat.NetworkMessage;

public class AddMemberMessage : Message
{
    private static readonly byte[] Type = { (byte)MessageType.AddMember };
    
    public AddMemberMessage(MemberInfo member)
    {
        Member = member;
    }

    public MemberInfo Member { get; }
    
    public override byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        ms.Write(Type);
        ms.Write(Member.ToBytes());
        return ms.ToArray();
    }

    protected bool Equals(AddMemberMessage other)
    {
        return Member.Equals(other.Member);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AddMemberMessage)obj);
    }

    public override int GetHashCode()
    {
        return Member.GetHashCode();
    }
}
