using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeShuiLa
{
    public class AppService : ObservableObject
    {
        private static AppService instance;
        public static AppService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppService();
                }
                return instance;
            }
        }

        private readonly List<string> hints = new List<string>();
        private readonly string appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "HeShuiLa");
        private readonly string hintsPath;
        private readonly string settingsPath;
        private readonly Random random = new Random();

        private string hintText = "Please drink water";
        public string HintText { get => hintText; set => SetProperty(ref hintText, value, nameof(HintText)); }

        private bool shouldUpdateHintText = true;
        public bool ShouldUpdateHintText { get => shouldUpdateHintText; set => SetProperty(ref shouldUpdateHintText, value, nameof(ShouldUpdateHintText)); }

        private int reminderInterval = 30 * 60 * 1000; // 30 minutes
        public int ReminderInterval
        {
            get => reminderInterval;
            set => SetProperty(ref reminderInterval, value, nameof(ReminderInterval));
        }

        private int reminderDuration = 30 * 1000; // 30 seconds
        public int ReminderDuration
        {
            get => reminderDuration;
            set => SetProperty(ref reminderDuration, value, nameof(ReminderDuration));
        }

        public int ReminderIntervalMinutes
        {
            get => ReminderInterval / (60 * 1000);
            set => ReminderInterval = value * 60 * 1000;
        }

        public int ReminderDurationSeconds
        {
            get => ReminderDuration / 1000;
            set
            {
                if (value < 5)
                {
                    value = 5;
                }
                if (value > ReminderIntervalMinutes * 60)
                {
                    value = ReminderIntervalMinutes * 60 / 2;
                }
                ReminderDuration = value * 1000;
            }
        }

        public event EventHandler SettingsChanged;

        private AppService()
        {
            hintsPath = Path.Combine(appDataPath, "hints.txt");
            settingsPath = Path.Combine(appDataPath, "settings.txt");
            EnsureAppDataDirectoryExists();
            LoadHints();
            LoadSettings();
        }

        private void EnsureAppDataDirectoryExists()
        {
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
        }

        private void LoadHints()
        {
            if (File.Exists(hintsPath))
            {
                hints.Clear();
                hints.AddRange(File.ReadAllLines(hintsPath).Where(h => !string.IsNullOrWhiteSpace(h)));
            }
            if (!hints.Any())
            {
                hints.Add("请喝水~");
                hints.Add("喝水时间到");
                hints.Add("人体70% 都是水，不喝水会变木乃伊");
                hints.Add("水是生命之源");
                hints.Add("杯子里有水不喝要浇花吗");
                hints.Add("因为热爱，所以喝水");
                hints.Add("咋的这水咬人啊，一口都不喝");
                hints.Add("每天8杯，健康加倍");
                hints.Add("爱喝水的人运气不会太差");
                hints.Add("那水端着喝吧，别客气，饮水机里还有");
                SaveHints();
            }
        }

        private void SaveHints()
        {
            File.WriteAllLines(hintsPath, hints);
        }

        public void SaveSettings()
        {
            var settings = new[]
            {
                ShouldUpdateHintText.ToString(),
                ReminderInterval.ToString(),
                ReminderDuration.ToString()
            };
            File.WriteAllLines(settingsPath, settings);
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void LoadSettings()
        {
            if (File.Exists(settingsPath))
            {
                var settings = File.ReadAllLines(settingsPath);
                if (settings.Length >= 3)
                {
                    ShouldUpdateHintText = bool.Parse(settings[0]);
                    ReminderInterval = int.Parse(settings[1]);
                    ReminderDuration = int.Parse(settings[2]);
                }
            }
        }

        public async Task UpdateHintText()
        {
            if (hints.Count > 0)
            {
                int index = random.Next(hints.Count);
                HintText = hints[index];
            }
        }

        public IEnumerable<string> GetHints()
        {
            return hints.ToList();
        }

        public void UpdateHints(string[] newHints)
        {
            hints.Clear();
            hints.AddRange(newHints);
            SaveHints();
        }
    }
}
