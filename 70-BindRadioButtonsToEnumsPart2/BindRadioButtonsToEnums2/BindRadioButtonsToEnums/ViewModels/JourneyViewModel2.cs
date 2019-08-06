using BindRadioButtonsToEnums.Common;
using BindRadioButtonsToEnums.Helpers;
using BindRadioButtonsToEnums.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BindRadioButtonsToEnums.ViewModels
{
    public sealed class JourneyViewModel2 : BindableBase
    {
        private TransportationMode selectedTransportationMode = TransportationMode.Rickshaw;
        public TransportationMode SelectedTransportationMode
        {
            get { return this.selectedTransportationMode; }
            set
            {
                if (this.SetProperty(ref this.selectedTransportationMode, value))
                {
                    this.OnPropertyChanged("IsCar");
                    this.OnPropertyChanged("IsBicycle");
                    this.OnPropertyChanged("IsTrain");
                    this.OnPropertyChanged("IsBoat");
                    this.OnPropertyChanged("IsRickshaw");
                    this.OnPropertyChanged("IsHovercraft");
                    this.OnPropertyChanged("EstimatedDuration");
                }
            }
        }

        public bool IsCar 
        {
            get { return this.SelectedTransportationMode == TransportationMode.Car; }
            set { if (value) this.SelectedTransportationMode = TransportationMode.Car; }
        }

        public bool IsBicycle
        {
            get { return this.SelectedTransportationMode == TransportationMode.Bicycle; }
            set { if (value) this.SelectedTransportationMode = TransportationMode.Bicycle; }
        }

        public bool IsTrain
        {
            get { return this.SelectedTransportationMode == TransportationMode.Train; }
            set { if (value) this.SelectedTransportationMode = TransportationMode.Train; }
        }

        public bool IsBoat
        {
            get { return this.SelectedTransportationMode == TransportationMode.Boat; }
            set { if (value) this.SelectedTransportationMode = TransportationMode.Boat; }
        }

        public bool IsRickshaw
        {
            get { return this.SelectedTransportationMode == TransportationMode.Rickshaw; }
            set { if (value) this.SelectedTransportationMode = TransportationMode.Rickshaw; }
        }

        public bool IsHovercraft
        {
            get { return this.SelectedTransportationMode == TransportationMode.Hovercraft; }
            set { if (value) this.SelectedTransportationMode = TransportationMode.Hovercraft; }
        }

        public string EstimatedDuration
        {
            get { return string.Format("{0} minutes", 13 + 17 * (int)SelectedTransportationMode); }
        }

        public ICommand GoByBicycleCommand
        {
            get
            {
                return new DelegateCommand(delegate
                    {
                        this.SelectedTransportationMode = TransportationMode.Bicycle;
                    });
            }
        }
    }
}
