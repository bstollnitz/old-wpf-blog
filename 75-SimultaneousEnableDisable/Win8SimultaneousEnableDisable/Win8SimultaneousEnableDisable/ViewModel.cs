using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Win8SimultaneousEnableDisable
{
    public class ViewModel : INotifyPropertyChanged
    {
        private bool isAvailable = true;
        public bool IsAvailable
        {
            get { return isAvailable; }
            set
            {
                if (SetField(ref isAvailable, value))
                {
                    doWorkCommand.CanExecute = isAvailable;
                }
            }
        }

        private string status = "Ready";
        public string Status
        {
            get { return status; }
            set { SetField(ref status, value); }
        }

        private bool useBasicOptions = true;
        public bool UseBasicOptions
        {
            get { return useBasicOptions; }
            set { SetField(ref useBasicOptions, value); }
        }

        private bool useExtraOptions = true;
        public bool UseExtraOptions
        {
            get { return useExtraOptions; }
            set { SetField(ref useExtraOptions, value); }
        }

        private DelegateCommand doWorkCommand;
        public ICommand DoWorkCommand
        {
            get
            {
                if (doWorkCommand == null)
                {
                    doWorkCommand = new DelegateCommand(DoWork);
                }
                return doWorkCommand;
            }
        }

        private async void DoWork()
        {
            IsAvailable = false;
            for (int i = 0; i < 5; i++)
            {
                Status = String.Format("Working on step {0}...", i + 1);
                await Task.Delay(1000);
            }
            IsAvailable = true;
            Status = "Done";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetField<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Object.Equals(storage, value))
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            System.Diagnostics.Debug.Assert(String.IsNullOrEmpty(propertyName) ||
                GetType().GetTypeInfo().DeclaredProperties.Any(p => p.Name == propertyName));
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
