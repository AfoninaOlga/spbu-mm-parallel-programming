using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Task5;

namespace Task5Ui
{
	internal class ViewModel : INotifyPropertyChanged
	{
		private ChatClient? client;
		private UserInfo myUserInfo;

		private string errorMessage = String.Empty;
		public string ErrorMessage
		{
			get => errorMessage;

			set
			{
				errorMessage = value;
				OnPropertyChanged(nameof(ErrorMessage));
			}
		}

		private Screen currentScreen;
		public Screen CurrentScreen
		{
			get => currentScreen;

			set
			{
				currentScreen = value;
				OnPropertyChanged(nameof(CurrentScreen));
			}
		}

		private bool isRunCommandAvailable = true;
		public string MyUserName { get; set; } = "";
		public string MyPort { get; set; } = "";
		public string PeerEndPoint { get; set; } = "";

		public Command RunCommand { get; private set; }

		private string currentMessage = "";
		public string CurrentMessage
		{
			get => currentMessage;
			set
			{
				currentMessage = value;
				OnPropertyChanged(nameof(CurrentMessage));
			}
		}

		public ObservableCollection<Message> Messages { get; set; } = new();

		public Command SendMessageCommand { get; private set; }

		public ViewModel()
		{
			RunCommand = new Command(Run, () => isRunCommandAvailable);
			SendMessageCommand = new Command(SendMessage, () => true);
		}

		public void Run()
		{
			if (MyUserName.Contains('$') || MyUserName == string.Empty)
			{
				ErrorMessage = "User name is empty or contains forbidden symbols ('$')";
				return;
			}

			if (!int.TryParse(MyPort, out var myPort) || myPort is < 1 or > 65535)
			{
				ErrorMessage = "Port should be an integer number from 1 to 65535";
				return;
			}

			IPEndPoint? peerEndPoint = null;

			if (PeerEndPoint != string.Empty)
			{
				if (!IPEndPoint.TryParse(PeerEndPoint, out peerEndPoint))
				{
					ErrorMessage = "Peer endpoint should be in format I.I.I.I:P";
					return;
				}
			}

			myUserInfo = new UserInfo(MyUserName);

			try
			{
				isRunCommandAvailable = false;

				client = new ChatClient(myUserInfo, myPort);
				client.MessageReceived += OnNewMessage;
				client.NewUserConnected += OnUserConnected;

				if (peerEndPoint is null)
				{
					client.Start();
				}
				else
				{
					Task.Run(async () => await client.ConnectAsync(peerEndPoint));
				}

				CurrentScreen = Screen.Chat;
			}
			catch (Exception e)
			{
				isRunCommandAvailable = true;
#if DEBUG
				var message = $"Exception occured: {e}";
#else
				var message = $"Cannot run chat client";
#endif
				ErrorMessage = message;
			}

		}

		public void SendMessage()
		{
			if (CurrentMessage == string.Empty)
			{
				return;
			}
			var message = new Message(myUserInfo.Name, DateTime.Now, CurrentMessage, true, false);
			Messages.Add(message);

			Task.Run(async () => await client!.SendMessageAsync(message.Text));

			CurrentMessage = string.Empty;
		}

		private void OnNewMessage(object? sender, (UserInfo User, string Text) userAndText)
		{
			var message = new Message(userAndText.User.Name, DateTime.Now, userAndText.Text, false, false);
			App.Current.Dispatcher.Invoke(() => Messages.Add(message));
		}

		private void OnUserConnected(object? sender, UserInfo user)
		{
			var message = new Message(user.Name, DateTime.Now, $"Say 👋 to {user.Name}", false, true);
			App.Current.Dispatcher.Invoke(() => Messages.Add(message));
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged([CallerMemberName] string prop = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
	}
}
