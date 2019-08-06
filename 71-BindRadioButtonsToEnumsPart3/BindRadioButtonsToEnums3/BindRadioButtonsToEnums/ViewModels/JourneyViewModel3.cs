using BindRadioButtonsToEnums.Common;
using BindRadioButtonsToEnums.Helpers;
using BindRadioButtonsToEnums.Models;
using System.Windows.Input;

namespace BindRadioButtonsToEnums.ViewModels
{
    public sealed class JourneyViewModel3 : BindableBase
    {
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
