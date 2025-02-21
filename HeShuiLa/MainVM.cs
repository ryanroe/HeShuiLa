using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HeShuiLa
{
    public class MainVM : ObservableObject
    {
        public AppService App { get; } = AppService.Instance;
        public async Task UpdateHintText()
        {
            await App.UpdateHintText();
        }

        public event Action ShowReminderRequested;
        
        public void ShowReminder()
        {
            ShowReminderRequested?.Invoke();
        }
    }
}
