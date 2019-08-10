using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace MultiBindingConverter
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>

    public partial class Window1 : Window
    {

        public Window1()
        {
            InitializeComponent();
        }

    }

	public class Conv : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// Note that we have to downcast each of the values (an object) to its actual
			// type (double) before converting it to a float.
			float alpha = (float)(double)(values[0]);
			float red = (float)(double)(values[1]);
			float green = (float)(double)(values[2]);
			float blue = (float)(double)(values[3]);

			return Color.FromScRgb(alpha, red, green, blue);
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException("ConvertBack should never be called");
		}
	}
}