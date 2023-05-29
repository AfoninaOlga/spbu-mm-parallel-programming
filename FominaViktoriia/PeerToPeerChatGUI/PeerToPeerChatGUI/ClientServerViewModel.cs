using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PeerToPeerChat;

namespace PeerToPeerChatGUI
{
    internal class ClientServerViewModel : IDisposable, INotifyPropertyChanged
    {
        private readonly ClientServer _clientServer = new();

        private readonly int _bufferSize = 1024;

        internal ObservableCollection<string> MessagesHistory { get; private set; } = new();

        internal delegate void ShowErrorMessage(object sender, string message);

        internal event ShowErrorMessage ThrowError = (_, __) => { };

        public event PropertyChangedEventHandler? PropertyChanged;

        internal string Host { get; set; } = "192.168.0.5";

        internal int Port { get; set; } = 8000;

        internal string Message { get; set; } = "";

        internal async Task ConnectAsync()
        {
            try
            {
                var ipIsParsed = IPAddress.TryParse(Host, out var ipAddress);

                if (!ipIsParsed)
                {
                    ThrowError(this, $"IP address [{ipAddress}] cannot be parsed");
                }

                var ipEndPoint = new IPEndPoint(ipAddress!, Port);

                _clientServer.Connect(ipEndPoint!);
                MessageBox.Show($"Connected to {ipEndPoint}\n");
            }
            catch (Exception e)
            {
                var ipIsParsed = IPAddress.TryParse(Host, out var ipAddress);
                var ipEndPoint = new IPEndPoint(ipAddress!, Port);
                ThrowError(this, $"Tried to connect to {ipEndPoint}" + e.Message);
            }
        }

        internal void SendAsync()
        {
            try
            {
                _clientServer.Send(Encoding.UTF8.GetBytes(Message));
                MessagesHistory.Add(Message);
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }            
        }

        internal void ReceiveAsync()
        {
            while (true)
            {
                var buffer = new byte[_bufferSize];
                _clientServer.Receive(buffer);

                var message = Encoding.UTF8.GetString(buffer);
                MessagesHistory.Add(message);
                Task.Delay(1000).Wait();
            }
        }

        public void Dispose() => _clientServer.Dispose();
    }
}
