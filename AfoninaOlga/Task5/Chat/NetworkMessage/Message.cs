namespace Chat.NetworkMessage;

public abstract class Message
{
    public enum MessageType : byte
    {
        Connect = 0,
        Members = 1,
        AddMember = 2,
        User = 3,
        Disconnect = 4,
        
        Echo = 5,
    }
    
    public abstract byte[] ToBytes();
}
