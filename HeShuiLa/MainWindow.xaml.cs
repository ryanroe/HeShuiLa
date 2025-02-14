using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using Forms = System.Windows.Forms;
using System.Windows.Threading;

namespace HeShuiLa
{
    public partial class MainWindow : Window
    {
        private const int REMIDER_INTERVAL = 30 * 60 * 1000; // 0.5 hour
        private const int REMIDER_DURATION = 60 * 1000; // 1 minute
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
            this.Hide();
            this.ShowInTaskbar = false;

        }

        private void InitializeTimer()
        {
            reminderTimer = new DispatcherTimer();
            reminderTimer.Interval = TimeSpan.FromSeconds(REMIDER_INTERVAL);
            reminderTimer.Tick += ReminderTimer_Tick;
            reminderTimer.Start();
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon(System.Drawing.SystemIcons.Information, 40, 40);
            notifyIcon.Visible = true;
            notifyIcon.Text = "喝水啦";

            var contextMenu = new Forms.ContextMenuStrip();
            var exitItem = new Forms.ToolStripMenuItem("退出");
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
                this.Topmost = true;

                var showStoryboard = (Storyboard)FindResource("ShowAnimation");
                showStoryboard.Completed += async (s, e) =>
                {
                    await Task.Delay(REMIDER_DURATION);
                    Dispatcher.Invoke(HideWithAnimation);
                };
                showStoryboard.Begin(this);
            }
        }

        private void HideWithAnimation()
        {
            var hideStoryboard = (Storyboard)FindResource("HideAnimation");
            hideStoryboard.Completed += (s, e) =>
            {
                this.Hide();
                isShowing = false;
            };
            hideStoryboard.Begin(this);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (isExiting)
            {
                notifyIcon.Dispose();
                base.OnClosing(e);
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
