namespace Chat.NetworkMessage;

public class DisconnectMessage : Message
{
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
