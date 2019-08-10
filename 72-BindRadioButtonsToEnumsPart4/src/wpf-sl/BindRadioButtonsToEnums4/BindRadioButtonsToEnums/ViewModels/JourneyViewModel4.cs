using BindRadioButtonsToEnums.Common;
using BindRadioButtonsToEnums.Helpers;
using BindRadioButtonsToEnums.Models;
using System.Windows.Input;

namespace BindRadioButtonsToEnums.ViewModels
{
    public sealed class JourneyViewModel4 : BindableBase
    {
        public JourneyViewModel4()
        {
            this.SelectedTransportationMode = new BindableEnum<TransportationMode>(TransportationMode.Rickshaw);
            this.SelectedTransportationMode.ValueChanged += delegate
            {
                this.OnPropertyChanged("EstimatedDuration");
            };
        }

        public BindableEnum<TransportationMode> SelectedTransportationMode { get; private set; }

        public string EstimatedDuration
        {
            get { return string.Format("{0} minutes", 13 + 17 * (int)SelectedTransportationMode.Value); }
        }

        public ICommand GoByBicycleCommand
        {
            get
            {
                return new DelegateCommand(delegate
                {
                    this.SelectedTransportationMode.Value = TransportationMode.Bicycle;
                });
            }
        }
    }
}
