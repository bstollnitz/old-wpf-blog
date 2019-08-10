using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfSimultaneousEnableDisable
{
    public sealed class DelegateCommand : ICommand
    {
        private Action action;
        private bool canExecute;

        public DelegateCommand(Action action)
        {
            this.action = action;
            this.canExecute = true;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute
        {
            get { return canExecute; }
            set
            {
                if (canExecute != value)
                {
                    canExecute = value;
                    EventHandler handler = CanExecuteChanged;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return canExecute;
        }

        public void Execute(object parameter)
        {
            if (canExecute && action != null)
            {
                action();
            }
        }
    }
}
