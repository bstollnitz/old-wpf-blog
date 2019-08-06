using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace CustomControls
{
    public class FormattedRatioConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TextBlock currentElement = value as TextBlock;
            if (currentElement != null)
            {
                PieChartLabel label = TreeHelper.FindAncestor<PieChartLabel>(currentElement);
                if (label != null)
                {
                    return label.FormattedRatio;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
