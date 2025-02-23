﻿using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using Forms = System.Windows.Forms;
using System.Windows.Threading;

namespace HeShuiLa
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer countdownTimer;
        private DateTime nextReminderTime;
        private Forms.NotifyIcon notifyIcon;
        private bool isShowing = false;
        private bool isExiting = false;
        private MainVM vm;
        private readonly SettingsWindow settingsWindow = new SettingsWindow();

        public MainWindow()
        {
            InitializeComponent();
            this.vm = this.DataContext as MainVM;
            InitializeTimer();
            InitializeNotifyIcon();
            this.Opacity = 0;
            this.Hide();
            this.ShowInTaskbar = false;
            settingsWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            vm.App.SettingsChanged += App_SettingsChanged;
            vm.ShowReminderRequested += () => ShowWithAnimation(5000); // 5 seconds for test
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
            nextReminderTime = DateTime.Now.AddMilliseconds(vm.App.ReminderInterval);
            UpdateTrayIconText();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            UpdateTrayIconText();
            if (DateTime.Now >= nextReminderTime)
            {
                ShowWithAnimation(null); // Use configured duration for regular reminders
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
            notifyIcon.DoubleClick += (s, e) => OpenSettings();

            var contextMenu = new Forms.ContextMenuStrip();

            var settingsItem = new Forms.ToolStripMenuItem("设置");
            settingsItem.Click += (s, e) => OpenSettings();
            contextMenu.Items.Add(settingsItem);


            var exitItem = new Forms.ToolStripMenuItem("退出");
            exitItem.Click += (s, e) =>
            {
                isExiting = true;
                Application.Current.Shutdown();
            };
            contextMenu.Items.Add(exitItem);

            notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void ShowWithAnimation(int? customDuration = null)
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
                    await Task.Delay(customDuration ?? vm.App.ReminderDuration);
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
                if (vm.App.ShouldUpdateHintText)
                {
                    await vm.UpdateHintText();
                }
            };
            hideStoryboard.Begin(this);
        }

        private void OpenSettings()
        {
            if (!settingsWindow.IsVisible)
                settingsWindow.ShowDialog();
        }

        private void App_SettingsChanged(object sender, EventArgs e)
        {
            if (!isShowing)
            {
                countdownTimer.Stop();
                ScheduleNextReminder();
                countdownTimer.Start();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (isExiting)
            {
                vm.App.SettingsChanged -= App_SettingsChanged;
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
