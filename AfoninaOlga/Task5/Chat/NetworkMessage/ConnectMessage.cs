namespace Chat.NetworkMessage;

public class ConnectMessage : Message
{
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
