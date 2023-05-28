using System.Net.Sockets;

namespace Chat.NetworkMessage;

public class MessageReader
{
    private readonly Socket _socket;

    public MessageReader(Socket socket)
    {
        _socket = socket;
    }
    
    public Message ReadMessage()
    {
        var messageType = ReadBytes(1);
        return (Message.MessageType)messageType[0] switch
        {
            Message.MessageType.Connect => new ConnectMessage( ReadMemberInfo()),
            Message.MessageType.Members => new MembersMessage( ReadMembers()),
            Message.MessageType.AddMember => new AddMemberMessage( ReadMemberInfo()),
            Message.MessageType.User => new UserMessage(ReadString(), ReadString()),
            Message.MessageType.Disconnect => new DisconnectMessage( ReadMemberInfo()),
            Message.MessageType.Echo => new EchoMessage(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private MemberInfo ReadMemberInfo()
    {
        var username = ReadString();
        var ip = ReadString();
        var port = ReadInt();
        return new MemberInfo(username, ip, port);
    }

    private MemberInfo[] ReadMembers()
    {
        var length = ReadInt();
        var members = new MemberInfo[length]; 
        for (var i = 0; i < length; i++)
        {
            members[i] = ReadMemberInfo();
        }
        return members;
    }

    private int ReadInt()
    {
        var bytes = ReadBytes(sizeof(int));
        return BitConverter.ToInt32(bytes);
    }

    private string ReadString()
    {
        var length = ReadInt();
        var bytes = ReadBytes(length);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    private byte[] ReadBytes(int n)
    {
        var buffer = new byte[n];
        var readBytes = 0;
        while (readBytes != n)
        {
            readBytes += _socket.Receive(buffer);
        }

        return buffer;
    }
}
