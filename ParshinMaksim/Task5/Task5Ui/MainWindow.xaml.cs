using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace Task5Ui
{
	public partial class MainWindow : Window
	{
		private ViewModel viewModel = new ViewModel();

		public MainWindow()
		{
			InitializeComponent();
			DataContext = viewModel;
			viewModel.PropertyChanged += OnPropertyChanged;
			viewModel.Messages.CollectionChanged += OnMessagesChanged;
			LoginScreen.Visibility = Visibility.Visible;
			ChatScreen.Visibility = Visibility.Hidden;
		}

		private void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs args)
		{
			MessageList.ScrollIntoView(MessageList.Items[^1]);
		}

		private void OnPropertyChanged(object? sender, PropertyChangedEventArgs args)
		{
			if (args.PropertyName is nameof(viewModel.ErrorMessage))
			{
				System.Windows.MessageBox.Show(viewModel.ErrorMessage);
			}
			else if (args.PropertyName is nameof(viewModel.CurrentScreen))
			{
				switch (viewModel.CurrentScreen)
				{
					case Screen.Chat:
						{
							LoginScreen.Visibility = Visibility.Hidden;
							ChatScreen.Visibility = Visibility.Visible;
							break;
						}
					case Screen.Login:
						{
							LoginScreen.Visibility = Visibility.Visible;
							ChatScreen.Visibility = Visibility.Hidden;
							break;
						}
				}
			}
			else if (args.PropertyName is nameof(viewModel.CurrentMessage))
			{
				if (viewModel.CurrentMessage == string.Empty)
				{
					MessageBox.Clear();
				}
			}
		}
	}
}
