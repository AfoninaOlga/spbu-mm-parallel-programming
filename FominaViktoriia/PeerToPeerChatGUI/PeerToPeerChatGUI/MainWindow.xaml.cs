using System.Windows;

namespace PeerToPeerChatGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ClientServerViewModel _viewModel;

        public MainWindow()
        {
            _viewModel = new ClientServerViewModel();
            _viewModel.ThrowError += (sender, message) => ShowMessage(message);

            DataContext = _viewModel;
            InitializeComponent();
            _viewModel.ReceiveAsync();
        }

        private async void ConnectAsync_Click(object sender, RoutedEventArgs e) =>
                await _viewModel.ConnectAsync();

        private async void SendAsync_Click(object sender, RoutedEventArgs e) =>
                await _viewModel.SendAsync();

        private static void ShowMessage(string errorMessage) =>
                MessageBox.Show(errorMessage, "Error message");
    }
}
