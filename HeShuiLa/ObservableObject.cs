using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeShuiLa
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SetProperty<T>(ref T value, T newValue, string name)
        {
            if (value != null && newValue != null && value.Equals(newValue)) return;
            value = newValue;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        protected void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
