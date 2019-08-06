using System;
using System.Windows.Input;

namespace BetterBindableBaseSample
{
    public class DelegateCommand<T> : ICommand
    {
        private Action<T> _action;
        private Func<T, bool> _canExecute;

        public DelegateCommand(Action<T> action, Func<T, bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute((T)parameter) : true;
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }

    public sealed class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action action, Func<bool> canExecute = null)
            : base(_ => action(), _ => canExecute != null ? canExecute() : true)
        {
        }
    }
}