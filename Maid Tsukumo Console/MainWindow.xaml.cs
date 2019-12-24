using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Maid_Tsukumo_Console
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ContentRendered += (s, e) => Loading();

            TextCompositionManager.AddPreviewTextInputHandler(this,OnTextInput);
            TextCompositionManager.AddPreviewTextInputUpdateHandler(this,TextInputUpdate);

            log = new List<(string, string)>();
            log.Add((string.Empty, string.Empty));
            Input.KeyUp += KeyUp;
        }

        private FileStream Stream;
        private FileSystemWatcher watcher;
        protected void Loading()
        {
            State.Text = "Loading Logs";
            Re();
            Stream = new FileStream($@"{Environment.CurrentDirectory}\書庫\履歴\system\{DateTime.Now.ToString("yyyy-MM-dd")}.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            Logger.Text = new StreamReader(Stream).ReadToEnd();

            State.Text = "Loading File Watcher";
            Re();
            watcher = new FileSystemWatcher();

            State.Text = "Set Target Directory";
            Re();
            watcher.Path = Path.GetDirectoryName($@"{Environment.CurrentDirectory}\書庫\履歴\system\{DateTime.Now.ToString("yyyy-MM-dd")}.txt");

            State.Text = "Set Target Filter";
            Re();
            watcher.Filter = Path.GetFileName($@"{Environment.CurrentDirectory}\書庫\履歴\system\{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
            watcher.NotifyFilter = NotifyFilters.LastWrite;

            State.Text = "Set Event Handler";
            Re();
            watcher.Changed += new FileSystemEventHandler(Changed);
            watcher.EnableRaisingEvents = true;
        }

        protected void Changed(object sender, FileSystemEventArgs e)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                State.Text = "Loading Logs";
                Re();
                Stream = new FileStream($@"{Environment.CurrentDirectory}\書庫\履歴\system\{DateTime.Now.ToString("yyyy-MM-dd")}.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                Logger.Text = new StreamReader(Stream).ReadToEnd();
            }));
        }

        private MainWindow Re()
        {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj =>
            {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            });
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
            Scroll.ScrollToEnd();
            return this;
        }

        private bool IsConvert = false;
        private int EnterBuffer { get; set; }
        private List<(string, string)> log;
        private int logdex = 0;
        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            EnterBuffer = IsConvert ? 1 : 0;
            IsConvert = false;
        }
        private void TextInputUpdate(object sender, TextCompositionEventArgs e) => IsConvert = e.TextComposition.CompositionText.Length == 0 ? false : true;
        private void KeyUp(object sender,KeyEventArgs e)
        {
            if (!IsConvert && e.Key == Key.Enter)
            {
                if (EnterBuffer == 1)
                    EnterBuffer = 0;
                else if (EnterBuffer == 0)
                {
                    State.Text = $"Sending Request <{Type.Text + Input.Text}>";
                    Re();
                    byte[] b = Encoding.UTF8.GetBytes(Type.Text + Input.Text + '\x1a');
                    Task.Run(() =>
                    {
                        TcpClient tcp = new TcpClient("localhost", 23145);
                        NetworkStream network = tcp.GetStream();
                        network.WriteAsync(b, 0, b.Length);
                        network.Close();
                        tcp.Close();
                    });
                    log.Add((Type.Text, Input.Text));
                    logdex = log.Count - 1;
                    Input.Text = string.Empty;
                }
            }
            else if (e.Key == Key.Up && logdex != 0)
            {
                logdex--;
                (Type.Text, Input.Text) = log[logdex];
                Re();
            }
            else if (e.Key == Key.Down && logdex != log.Count - 1)
            {
                logdex++;
                (Type.Text, Input.Text) = log[logdex];
                Re();
            }
        }
    }
}
