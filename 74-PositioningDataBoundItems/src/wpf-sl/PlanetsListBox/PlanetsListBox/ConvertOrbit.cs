using System;
using Windows.UI.Xaml.Data;

namespace PlanetsListBox
{
	public class ConvertOrbit : IValueConverter
	{
        public object Convert(object value, Type targetType, object parameter, string language)
		{
			double orbit = (double)value;
			double factor = System.Convert.ToDouble(parameter);
			return Math.Pow(orbit / 40, 0.4) * 770 * factor;
		}

        public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException("This method should never be called");
		}
    }
}
