using System.Net;
using System.Net.Sockets;

namespace Network;

public static class Utils
{
    public static int FindFreePort()
    {
        var port = 0;
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Any, 0));
        var localEp = (IPEndPoint?)socket.LocalEndPoint;
        if (localEp != null) port = localEp.Port;
        return port;
    }
}
