using System;
using System.Windows.Input;

namespace BindRadioButtonsToEnums.Helpers
{
    public sealed class DelegateCommand : ICommand
    {
        private Action action;

        public DelegateCommand(Action action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67

        public void Execute(object parameter)
        {
            this.action();
        }
    }
}
