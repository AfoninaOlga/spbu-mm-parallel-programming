using System.Threading.Tasks;
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

            var task = new Task(() =>
            {
                _viewModel.Receive();
            });
            task.Start();
        }

        private void ConnectAsync_Click(object sender, RoutedEventArgs e) => _viewModel.Connect();

        private void SendAsync_Click(object sender, RoutedEventArgs e) =>
                _viewModel.Send();

        private static void ShowMessage(string errorMessage) =>
                MessageBox.Show(errorMessage, "Error message");
    }
}
