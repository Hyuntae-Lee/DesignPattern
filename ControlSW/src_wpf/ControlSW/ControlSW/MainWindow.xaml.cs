using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace ControlSW
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CommMngr _comm;
        private ObservableCollection<string> _messages = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();

            MessagesList.ItemsSource = _messages;

            _comm = new CommMngr();
            _comm.OnConnectionStatusChanged = OnConnectionStatusChanged;
            _comm.OnRespReceive = (m) => AddMessage($"RESP: {m}");
            _comm.OnCommandReceived = (m) => AddMessage($"CMD: {m}");
            _comm.OnCommentReceived = (m) => AddMessage($"CMT: {m}");
        }

        private void OnConnectionStatusChanged(bool connected)
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = connected ? "Connected" : "Disconnected";
                BtnOpenClose.Content = connected ? "Close Connection" : "Open Connection";
            });
        }

        private void AddMessage(string msg)
        {
            Dispatcher.Invoke(() =>
            {
                _messages.Insert(0, $"[{DateTime.Now:HH:mm:ss}] {msg}");
            });
        }

        private void BtnOpenClose_Click(object sender, RoutedEventArgs e)
        {
            if (_comm.IsConnected())
            {
                _comm.CloseConnection();
                return;
            }

            var ip = IpText.Text.Trim();
            if (!int.TryParse(PortText.Text.Trim(), out int port))
            {
                MessageBox.Show("Invalid port");
                return;
            }

            var ok = _comm.OpenConnection(ip, port);
            if (!ok)
            {
                MessageBox.Show("Failed to open connection");
            }
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (!_comm.IsConnected())
            {
                MessageBox.Show("Not connected");
                return;
            }

            var text = SendText.Text;
            if (string.IsNullOrEmpty(text)) return;

            bool wait = WaitForRespBox.IsChecked == true;
            try
            {
                _comm.SendCommand(text, wait);
                AddMessage($"SENT: {text}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Send failed: {ex.Message}");
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _messages.Clear();
        }
    }
}
