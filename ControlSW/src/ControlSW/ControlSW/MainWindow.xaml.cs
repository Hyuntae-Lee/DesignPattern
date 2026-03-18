using CommMngrLib;
using System;
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
        private bool IsConnected;

        public MainWindow()
        {
            InitializeComponent();
            _comm = new CommMngr();
            _comm.RespReceived += OnRespReceived;
            _comm.CommentReceived += OnCommentReceived;
            _comm.CommandReceived += OnCommandReceived;

            IsConnected = _comm.OpenConnection("127.0.0.1", 31190);

            _comm.SendCommand("[SPM_VOLU_30]", true);
        }

        private void OnCommandReceived(object sender, string msg)
        {
            // Ensure UI updates happen on UI thread
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                // Prepend timestamp for readability
                var display = $"{DateTime.Now:HH:mm:ss} {msg}";
                lvCommands.Items.Insert(0, display);
            }));
        }

        private void OnRespReceived(object sender, string msg)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var display = $"{DateTime.Now:HH:mm:ss} {msg}";
                lvResponses.Items.Insert(0, display);
            }));
        }

        private void OnCommentReceived(object sender, string msg)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                var display = $"{DateTime.Now:HH:mm:ss} {msg}";
                lvComments.Items.Insert(0, display);
            }));
        }
    }
}
