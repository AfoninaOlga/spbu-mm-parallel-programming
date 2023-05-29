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

        public ClientServer()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            var endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8000);
            _socket.Bind(endPoint);
            _socket.Listen();

            _ipEndPoints = new();
        }

        public void Receive(byte[] buffer)
        {
            var socket = _socket.Accept();
            socket.Receive(buffer);

            var ipEndPoint = ParseIpAndPort(buffer);

            if (ipEndPoint != null && !_ipEndPoints.Contains(ipEndPoint))
            {                
                foreach (var iep in _ipEndPoints)
                {
                    var endPoint = $"ip:{iep.Address};port:{iep.Port}";
                    var endPointBytes = Encoding.UTF8.GetBytes(endPoint);
                    socket.Send(endPointBytes);
                }

                _ipEndPoints.Add(ipEndPoint);
            }

            socket.Close();
        }

        public void Send(byte[] buffer)
        {
            foreach (var iep in _ipEndPoints)
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                socket.Connect(iep);
                socket.Send(buffer);
                Task.Delay(1000);
                socket.Close();
            }
        }

        public void Connect(IPEndPoint endPoint)
        {
            _ipEndPoints.Add(endPoint);
        }

        public void Close() => _socket.Close();

        public void Dispose() => _socket.Dispose();

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