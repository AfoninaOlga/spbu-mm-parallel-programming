namespace Chat.NetworkMessage;

public class MemberInfo
{
    public MemberInfo(string username, string ip, int port)
    {
        Username = username;
        Ip = ip;
        Port = port;
    }

    public string Username { get; }
    public string Ip { get; }
    public int Port { get; }

    public byte[] ToBytes()
    {
        using var ms = new MemoryStream();
        ms.Write(BitConverter.GetBytes(Username.Length));
        ms.Write(System.Text.Encoding.UTF8.GetBytes(Username));
        ms.Write(BitConverter.GetBytes(Ip.Length));
        ms.Write(System.Text.Encoding.UTF8.GetBytes(Ip));
        ms.Write(BitConverter.GetBytes(Port));
        return ms.ToArray();
    }
}
