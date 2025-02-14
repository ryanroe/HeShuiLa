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
using System.Windows.Threading;
using System.Windows.Media.Animation;
using Forms = System.Windows.Forms;

namespace HeShuiLa
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer reminderTimer;
        private Forms.NotifyIcon notifyIcon;
        private bool isShowing = false;
        private bool isExiting = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeNotifyIcon();
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            
            // Only prevent closing if not exiting through tray icon
            this.Closing += (s, e) => e.Cancel = !isExiting;
        }

        private void InitializeTimer()
        {
            reminderTimer = new DispatcherTimer();
            reminderTimer.Interval = TimeSpan.FromSeconds(60); 
            reminderTimer.Tick += ReminderTimer_Tick;
            reminderTimer.Start();
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon(System.Drawing.SystemIcons.Information, 40, 40);
            notifyIcon.Visible = true;
            notifyIcon.Text = "Water Reminder";

            // Create context menu
            var contextMenu = new Forms.ContextMenuStrip();
            var exitItem = new Forms.ToolStripMenuItem("Exit");
            exitItem.Click += (s, e) => 
            {
                isExiting = true;
                Application.Current.Shutdown();
            };
            contextMenu.Items.Add(exitItem);

            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
            ShowWithAnimation();
        }

        private void ShowWithAnimation()
        {
            if (!isShowing)
            {
                isShowing = true;
                this.Show();
                this.Activate();
                this.Topmost = true; // Make sure window stays on top

                var storyboard = new Storyboard();

                // Opacity animation
                var fadeIn = new DoubleAnimation
                {
                    From = 0.0,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(1)
                };
                Storyboard.SetTarget(fadeIn, this);
                Storyboard.SetTargetProperty(fadeIn, new PropertyPath(OpacityProperty));

                // Background color animation
                var colorAnimation = new ColorAnimation
                {
                    From = Colors.Transparent,
                    To = Color.FromArgb(204, 0, 0, 0), // #CC000000
                    Duration = TimeSpan.FromSeconds(1)
                };
                
                this.Background = new SolidColorBrush(Color.FromArgb(204, 0, 0, 0));
                Storyboard.SetTarget(colorAnimation, this.Background);
                Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));

                storyboard.Children.Add(fadeIn);
                storyboard.Children.Add(colorAnimation);

                storyboard.Completed += (s, e) =>
                {
                    Task.Delay(30000).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(HideWithAnimation);
                    });
                };

                storyboard.Begin();
            }
        }

        private void HideWithAnimation()
        {
            var storyboard = new Storyboard();

            // Opacity animation
            var fadeOut = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(1)
            };
            Storyboard.SetTarget(fadeOut, this);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(OpacityProperty));

            // Background color animation
            var colorAnimation = new ColorAnimation
            {
                From = (this.Background as SolidColorBrush)?.Color,
                To = Colors.Transparent,
                Duration = TimeSpan.FromSeconds(1)
            };
            Storyboard.SetTarget(colorAnimation, this.Background);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath(SolidColorBrush.ColorProperty));

            storyboard.Children.Add(fadeOut);
            storyboard.Children.Add(colorAnimation);

            storyboard.Completed += (s, e) =>
            {
                this.Hide();
                isShowing = false;
            };

            storyboard.Begin();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (isExiting)
            {
                notifyIcon.Dispose();
                base.OnClosing(e);
            }
        }
    }
}
