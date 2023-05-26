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
}
