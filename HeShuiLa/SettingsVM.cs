using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;

namespace HeShuiLa
{
    public class SettingsVM : ObservableObject
    {
        public AppService App { get; } = AppService.Instance;
        public MainVM MainVM { get; }
        public ICommand SaveCommand { get; }
        public ICommand TestCommand { get; }

        private string hintsText;
        public string HintsText
        {
            get => hintsText;
            set => SetProperty(ref hintsText, value, nameof(HintsText));
        }

        private void Save()
        {
            App.UpdateHints(HintsText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            App.SaveSettings();
            MessageBox.Show("保存成功");
            Application.Current.Windows.OfType<SettingsWindow>().FirstOrDefault()?.Hide();
        }

        public SettingsVM()
        {
            MainVM = ((App)Application.Current).MainVM;
            SaveCommand = new DelegateCommand(s => Save(), s => true);
            TestCommand = new DelegateCommand(s => MainVM.ShowReminder(), s => true);
            HintsText = string.Join(Environment.NewLine, App.GetHints());
        }

    }
}
