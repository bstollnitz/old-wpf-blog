using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace VirtualizationWPF
{
    public class InstanceCounter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int value;

        // We need this to bind to it.
        public int Value
        {
            get
            {
                return this.value;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Increment()
        {
            Interlocked.Increment(ref this.value);
            OnPropertyChanged("Value");
        }

        public void Decrement()
        {
            Interlocked.Decrement(ref this.value);
            OnPropertyChanged("Value");
        }
    }
}
