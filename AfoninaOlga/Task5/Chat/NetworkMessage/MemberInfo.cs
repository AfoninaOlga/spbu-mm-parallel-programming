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

    protected bool Equals(MemberInfo other)
    {
        return Username == other.Username && Ip == other.Ip && Port == other.Port;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((MemberInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Username, Ip, Port);
    }

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
