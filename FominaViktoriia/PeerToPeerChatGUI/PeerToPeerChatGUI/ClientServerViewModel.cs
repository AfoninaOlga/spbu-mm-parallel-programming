using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        internal string Host { get; set; } = "127.0.0.1";

        internal int Port { get; set; } = 8000;

        internal string Message { get; set; } = "";

        internal async Task ConnectAsync()
        {
            var ipIsParsed = IPAddress.TryParse(Host, out var ipAddress);

            if (!ipIsParsed)
            {
                ThrowError(this, $"IP address [{ipAddress}] cannot be parsed");
            }

            var ipEndPoint = new IPEndPoint(ipAddress!, Port);

            await _clientServer.ConnectAsync(ipEndPoint!);
        }

        internal async Task SendAsync()
        {
            try
            {
                await _clientServer.SendAsync(Encoding.UTF8.GetBytes(Message));
                MessagesHistory.Add(Message);
            }
            catch (Exception e)
            {
                ThrowError(this, e.Message);
            }            
        }

        internal async Task<string> ReceiveAsync()
        {
            var buffer = new byte[_bufferSize];
            await _clientServer.ReceiveAsync(buffer);
            
            var message = Encoding.UTF8.GetString(buffer);
            MessagesHistory.Add(message);

            return message;
        }

        public void Dispose() => _clientServer.Dispose();
    }
}
