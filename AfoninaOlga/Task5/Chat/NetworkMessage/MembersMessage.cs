namespace Chat.NetworkMessage;

public class MembersMessage : Message
{
    private static readonly byte[] Type = { (byte)MessageType.Members };
    
    public MembersMessage(MemberInfo[] members)
    {
        Members = members;
    }

    public MemberInfo[] Members { get; }
    
    public override byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        ms.Write(Type);
        ms.Write(BitConverter.GetBytes(Members.Length));
        foreach (var member in Members)
        {
            ms.Write(member.ToBytes());
        }
        return ms.ToArray();
    }
}
