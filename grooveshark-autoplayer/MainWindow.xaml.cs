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
using System.Diagnostics;

namespace grooveshark_autoplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IHTMLDocument2 document;

        bool addedToolbar = false; //flag to check if the toolbar has been added as load completed can get called more than once
        bool autoPlay = false; //flag for autoplay/pause functionality

        AudioPlayingChecker checker; //class that is used to check if other stuff is playing
        DispatcherTimer timer; //timer to periodically check

        //flag to check whether the client is playing music. 
        //this is a bit of a stupid flag as it only changes when the toolbar buttons are used to play/pause grooveshark
        //todo: think of a better way to do this, maybe assume it's playing all the time? or even use audioplayingchecker
        //to check it through windows
        bool isPlaying = false; 

        public MainWindow()
        {
            InitializeComponent();
            checker = new AudioPlayingChecker();
        }

        //Pauses the grooveshark stream by sending a js command
        public void pause()
        {
            isPlaying = false;

            Object[] args = new Object[1];
            args[0] = (Object)"window.Grooveshark.pause();";
            browser.InvokeScript("eval", args);
            args = default(Object[]);

            Debug.WriteLine("Pause");
        }

        //Plays the grooveshark stream by sending a js command
        //Doesn't work if there's nothing in the queue to play already
        public void play()
        {
            isPlaying = true;

            Object[] args = new Object[1];
            args[0] = (Object)"window.Grooveshark.play();";
            browser.InvokeScript("eval", args);
            args = default(Object[]);

            Debug.WriteLine("Play");
        }

        private void BrowserLoadCompleted(object sender, NavigationEventArgs e)
        {
            //save a reference to the grooveshark page so we can send js calls to it later
            this.document = browser.Document as IHTMLDocument2; 

            setup_toolbar();

            //setup a timer. this timer checks if anything else is playing and pauses/plays grooveshark based on that
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500), IsEnabled = true };
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (!autoPlay)
                return;
            
            int playing = AudioPlayingChecker.NumberApplicationsPlaying();
            
            //Start playing if it's not playing and nothing else is playing
            //Pause if something else is playing and it is playing
            if (!isPlaying)
            {
                if (playing == 0)
                    this.play();
            }
            else
            {
                if (playing > 1)
                    this.pause();
            }
        }

        private void setup_toolbar()
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            autoPlay = !autoPlay;
        }


    }
}
