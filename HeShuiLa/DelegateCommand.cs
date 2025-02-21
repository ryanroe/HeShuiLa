using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HeShuiLa
{
    public class DelegateCommand : ICommand
    {

        public event EventHandler CanExecuteChanged;
        private readonly Action<object> action;
        private readonly Func<object, bool> canExecute;
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.action = execute;
            this.canExecute = canExecute;
        }
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public bool CanExecute(object parameter)
        {
            return canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            action.Invoke(parameter);
        }
    }
}
