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
        private const int REMIDER_INTERVAL = 30 * 60 * 1000; // 30 minutes
        private const int REMIDER_DURATION = 30 * 1000; // 1 minute
        private DispatcherTimer countdownTimer;
        private DateTime nextReminderTime;
        private Forms.NotifyIcon notifyIcon;
        private bool isShowing = false;
        private bool isExiting = false;
        private MainVM vm;
        private bool updateHintText = true; // Add this line

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeNotifyIcon();
            this.Opacity = 0;
            this.Hide();
            this.ShowInTaskbar = false;
            this.vm = this.DataContext as MainVM;
        }

        private void InitializeTimer()
        {
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += CountdownTimer_Tick;

            ScheduleNextReminder();
            countdownTimer.Start();
        }

        private void ScheduleNextReminder()
        {
            nextReminderTime = DateTime.Now.AddMilliseconds(REMIDER_INTERVAL);
            UpdateTrayIconText();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            UpdateTrayIconText();
            if (DateTime.Now >= nextReminderTime)
            {
                ShowWithAnimation();
            }
        }

        private void UpdateTrayIconText()
        {
            if (notifyIcon != null)
            {
                var timeRemaining = nextReminderTime - DateTime.Now;
                if (timeRemaining.TotalSeconds > 0)
                {
                    int minutes = (int)timeRemaining.TotalMinutes;
                    int seconds = (int)timeRemaining.TotalSeconds % 60;
                    notifyIcon.Text = $"喝水啦 - {minutes:00}:{seconds:00}";
                }
            }
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new Forms.NotifyIcon();
            notifyIcon.Icon = new System.Drawing.Icon(System.Drawing.SystemIcons.Information, 40, 40);
            notifyIcon.Visible = true;
            notifyIcon.Text = "喝水啦";

            var contextMenu = new Forms.ContextMenuStrip();

            var updateHintItem = new Forms.ToolStripMenuItem("更新提示语");
            updateHintItem.CheckOnClick = true;
            updateHintItem.Checked = updateHintText;
            updateHintItem.Click += (s, e) =>
            {
                updateHintText = updateHintItem.Checked;
            };
            contextMenu.Items.Add(updateHintItem);

            var exitItem = new Forms.ToolStripMenuItem("退出");
            exitItem.Click += (s, e) =>
            {
                isExiting = true;
                Application.Current.Shutdown();
            };
            contextMenu.Items.Add(exitItem);

            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void ShowWithAnimation()
        {
            if (!isShowing)
            {
                isShowing = true;
                countdownTimer.Stop();
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
            hideStoryboard.Completed += async (s, e) =>
            {
                this.Hide();
                isShowing = false;
                ScheduleNextReminder();
                countdownTimer.Start();
                if (updateHintText)
                {
                    await vm.UpdateHintText();
                }
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
