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
        private readonly string hintsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "hints.txt");
        private readonly Random random = new Random();

        private string hintText = "Please drink water";
        public string HintText { get => hintText; set => SetProperty(ref hintText, value, nameof(HintText)); }

        private bool shouldUpdateHintText = true;
        public bool ShouldUpdateHintText { get => shouldUpdateHintText; set => SetProperty(ref shouldUpdateHintText, value, nameof(ShouldUpdateHintText)); }

        private AppService()
        {
            LoadHints();
        }

        private void LoadHints()
        {
            if (File.Exists(hintsPath))
            {
                hints.Clear();
                hints.AddRange(File.ReadAllLines(hintsPath));
            }
            else
            {
                hints.Add("Please drink water");
                SaveHints();
            }
        }

        private void SaveHints()
        {
            File.WriteAllLines(hintsPath, hints);
        }

        public async Task UpdateHintText()
        {
            if (hints.Count > 0)
            {
                int index = random.Next(hints.Count);
                HintText = hints[index];
            }
        }
    }
}
