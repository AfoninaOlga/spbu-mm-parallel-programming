using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using PeerToPeerChat;

namespace PeerToPeerChatGUI
{
    public class ClientServerViewModel : IDisposable, INotifyPropertyChanged
    {
        private readonly ClientServer _clientServer = new();

        private readonly int _bufferSize = 1024;

        public ObservableCollection<string> MessagesHistory { get; private set; } = new();

        public delegate void ShowErrorMessage(object sender, string message);

        public event ShowErrorMessage ThrowError = (_, __) => { };

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string Host { get; set; } = "192.168.0.5";

        public int Port { get; set; } = 8000;

        public string Message { get; set; } = "";

        public void Connect()
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
                MessageBox.Show($"Added [{ipEndPoint}] to the list of hosts we will connect to\n");
            }
            catch (Exception e)
            {
                var ipIsParsed = IPAddress.TryParse(Host, out var ipAddress);
                var ipEndPoint = new IPEndPoint(ipAddress!, Port);
                ThrowError(this, e.Message);
            }
        }

        public void Send()
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

        public void Receive()
        {
            while (true)
            {
                var buffer = new byte[_bufferSize];
                _clientServer.Receive(buffer);

                var message = Encoding.UTF8.GetString(buffer);
                App.Current.Dispatcher.Invoke(() => MessagesHistory.Add(message));
            }
        }

        public void Dispose() => _clientServer.Dispose();
    }
}
