using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PeerToPeerChat
{
    public class ClientServer : IDisposable
    {
        private readonly Socket _socket;

        private readonly ConcurrentBag<IPEndPoint> _ipEndPoints;

        private readonly string _ip = "0.0.0.0";

        private readonly int _port = 8000;

        public ClientServer()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            var endPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);
            _socket.Bind(endPoint);
            _socket.Listen();

            _ipEndPoints = new();
        }

        public void Receive(byte[] buffer)
        {
            var socket = _socket.Accept();
            socket.Receive(buffer);

            var ipEndPoint = ParseIpAndPort(buffer);
            if (ipEndPoint != null && ipEndPoint.Address.ToString().Equals(_ip))
            {
                if (socket.RemoteEndPoint is IPEndPoint remoteIPEndPoint)
                {
                    ipEndPoint.Address = remoteIPEndPoint.Address;
                }
            }

            if (ipEndPoint != null && !_ipEndPoints.Contains(ipEndPoint))
            {
                Connect(ipEndPoint);
            }

            socket.Close();
        }

        public void Send(byte[] buffer)
        {
            if (_ipEndPoints.IsEmpty)
            {
                throw new InvalidOperationException("You did not specified the host to connect to");
            }

            foreach (var iep in _ipEndPoints)
            {
                SendToOne(buffer, iep);
            }
        }

        public void Connect(IPEndPoint ipEndPoint)
        {
            if (ipEndPoint == null)
            {
                throw new Exception("Attempt to connect to null IPEndPoint");
            }            

            foreach (var iep in _ipEndPoints)
            {
                var endPointBytes = Encoding.UTF8.GetBytes($"ip:{iep.Address};port:{iep.Port}");
                SendToOne(endPointBytes, ipEndPoint);
            }

            if (_socket.LocalEndPoint is IPEndPoint myiep)
            {
                var myEndPointBytes = Encoding.UTF8.GetBytes($"ip:{myiep.Address};port:{myiep.Port}");
                SendToOne(myEndPointBytes, ipEndPoint);
            }

            if (!_ipEndPoints.Contains(ipEndPoint))
            {
                _ipEndPoints.Add(ipEndPoint);
            }
        }

        public void Close() => _socket.Close();

        public void Dispose() => _socket.Dispose();

        private static void SendToOne(byte[] buffer, IPEndPoint iep)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            socket.Connect(iep);
            socket.Send(buffer);
            socket.Close();
        }

        private static IPEndPoint? ParseIpAndPort(byte[] buffer)
        {
            string acceptedString = Encoding.UTF8.GetString(buffer);

            if (acceptedString.Length > 16)
            {
                if (acceptedString[0] == 'i' && acceptedString[1] == 'p'
                        && acceptedString[2] == ':')
                {
                    var splitSttrings = acceptedString.Split(':', ';');

                    string ip = "";
                    string port = "";
                    foreach (var str in splitSttrings)
                    {
                        if (str.Contains('.'))
                        {
                            ip = str;
                        }
                        else if (char.IsDigit(str[0]))
                        {
                            port = str;
                        }
                    }

                    var ipIsParsed = IPAddress.TryParse(ip, out var ipAddress);

                    if (ipIsParsed)
                    {
                        var portIsParsed = int.TryParse(port, out var hostPort);

                        if (portIsParsed)
                        {
                            return new IPEndPoint(ipAddress!, hostPort);
                        }
                    }
                }
            }

            return null;
        }
    }
}