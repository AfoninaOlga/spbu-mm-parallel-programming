using System.Net;
using Chat.NetworkMessage;

namespace Chat;

using System.Net.Sockets;

public class Connection : IDisposable
{
    public static Connection NewConnection(IPEndPoint localEndPoint)
    {
        return new Connection(localEndPoint);
    }

    public static Connection ConnectTo(IPEndPoint remoteEndPoint)
    {   
        var clientSocket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(remoteEndPoint);
        clientSocket.Send(new EchoMessage().ToBytes());
        var localEndPoint = (IPEndPoint)clientSocket.LocalEndPoint!;
        clientSocket.Close();
        var connection = new Connection(new IPEndPoint(localEndPoint.Address, 0));
        connection.AddConnectedClient(remoteEndPoint);
        return connection;
    }

    private readonly Socket _listener;
    private Thread? _listenerThread;

    private readonly HashSet<IPEndPoint> _connectedClients = new();

    private Connection(IPEndPoint ipEndPoint)
    {
        _listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(ipEndPoint);
        LocalEndPoint = (IPEndPoint)_listener.LocalEndPoint;
        _listener.Listen();
    }

    public IPEndPoint LocalEndPoint { get; }

    public void StartListen(Action<Message> handleIncomeConnection, CancellationToken token)
    {
        _listenerThread = new Thread(() =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var socket = _listener.Accept();
                    var reader = new MessageReader(socket);
                    handleIncomeConnection(reader.ReadMessage());
                    socket.Close();
                }
                catch (SocketException)
                {
                    // Another try
                }
            }
        });
        _listenerThread.Start();
    }

    public void AddConnectedClient(IPEndPoint endPoint)
    {
        _connectedClients.Add(endPoint);
    }

    public void RemoveConnectedClient(IPEndPoint endPoint)
    {
        _connectedClients.Remove(endPoint);
    }

    public void SendTo(byte[] bytes, IPEndPoint endPoint)
    {
        var clientSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        clientSocket.Connect(endPoint);
        clientSocket.Send(bytes);
        clientSocket.Close();
    }

    public void SendBroadcast(byte[] bytes)
    {
        foreach (var connectedClient in _connectedClients)
        {
            var clientSocket = new Socket(connectedClient.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(connectedClient);
            clientSocket.Send(bytes);
            clientSocket.Close();
        }
    }

    public void Dispose()
    {
        _listener.Dispose();
        _connectedClients.Clear();
        _listenerThread?.Join();
    }
}
