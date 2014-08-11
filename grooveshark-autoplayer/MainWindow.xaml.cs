using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using mshtml;
using Microsoft.WindowsAPICodePack.Taskbar;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace grooveshark_autoplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IHTMLDocument2 document;
        bool addedToolbar = false;
        bool autoPlay = false;
        AudioPlayingChecker checker;
        DispatcherTimer timer;

        bool isPlaying = false;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        public MainWindow()
        {
            InitializeComponent();
            checker = new AudioPlayingChecker();
        }

        public void pause()
        {
            isPlaying = false;
            timer.IsEnabled = true;

            Object[] args = new Object[1];
            args[0] = (Object)"window.Grooveshark.pause();";
            browser.InvokeScript("eval", args);
            args = default(Object[]);
        }

        public void play()
        {
            isPlaying = true;
            timer.IsEnabled = false;

            Object[] args = new Object[1];
            args[0] = (Object)"window.Grooveshark.play();";
            browser.InvokeScript("eval", args);
            args = default(Object[]);
        }

        private void BrowserLoadCompleted(object sender, NavigationEventArgs e)
        {
            this.document = browser.Document as IHTMLDocument2;

            if (document == null) return;

            set_toolbar();

            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500), IsEnabled = true };
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (!checker.IsAudioPlaying() && !isPlaying && autoPlay)
            {
                this.play();
            }
        }

        private void set_toolbar()
        {
            if (addedToolbar)
                return;

            var pause = new ThumbnailToolbarButton(Properties.Resources.Pause, "Pause");
            pause.Click += (sender, args) => this.pause();

            var play = new ThumbnailToolbarButton(Properties.Resources.Play, "Play");
            play.Click += (sender, args) => this.play();

            if (TaskbarManager.IsPlatformSupported)
                TaskbarManager.Instance.ThumbnailToolbars.AddButtons(
                    new WindowInteropHelper(Application.Current.MainWindow).Handle,
                    pause,
                    play);

            addedToolbar = true;

        }

        private void setup()
        {
            
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            autoPlay = !autoPlay;
        }


    }
}
