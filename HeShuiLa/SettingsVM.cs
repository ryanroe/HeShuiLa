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
        public ICommand SaveCommand { get; }
        private void Save()
        {
            App.SaveSettings();
            MessageBox.Show("保存成功");
            Application.Current.Windows.OfType<SettingsWindow>().FirstOrDefault()?.Hide();
        }
        public SettingsVM()
        {
            SaveCommand = new DelegateCommand(s => Save(), s => true);
        }

    }
}
