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
    public sealed class JourneyViewModel1 : BindableBase
    {
        public JourneyViewModel1()
        {
            // If the order of the enum in the code is the same as you want in the UI.
            this.AvailableTransportationModes = Enum.GetValues(typeof(TransportationMode)).OfType<TransportationMode>().ToArray();

            // If you want the order of the items to be different in the UI.
            //this.AvailableTransportationModes = new TransportationMode[] { 
            //    TransportationMode.Train, 
            //    TransportationMode.Car, 
            //    TransportationMode.Rickshaw, 
            //    TransportationMode.Hovercraft, 
            //    TransportationMode.Bicycle, 
            //    TransportationMode.Boat 
            //};
        }

        public IEnumerable<TransportationMode> AvailableTransportationModes
        {
            get;
            private set;
        }

        private TransportationMode selectedTransportationMode = TransportationMode.Rickshaw;
        public TransportationMode SelectedTransportationMode
        {
            get { return this.selectedTransportationMode; }
            set
            {
                if (this.SetProperty(ref this.selectedTransportationMode, value))
                {
                    this.OnPropertyChanged("EstimatedDuration");
                }
            }
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
