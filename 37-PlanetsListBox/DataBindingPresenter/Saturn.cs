using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Presenter
{
    public class Saturn : INotifyPropertyChanged
    {
        private Uri imageFileName;

        public Uri ImageFileName
        {
            get { return imageFileName; }
            set
            {
                imageFileName = value;
                OnPropertyChanged("ImageFileName");
            }
        }

        private double temperature;

        public double Temperature
        {
            get { return temperature; }
            set 
            { 
                temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, 
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
