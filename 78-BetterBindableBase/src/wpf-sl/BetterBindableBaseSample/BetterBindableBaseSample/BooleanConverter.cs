using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BetterBindableBaseSample
{
    public abstract class BooleanConverter<T> : IValueConverter
    {
        protected BooleanConverter(T trueValue, T falseValue)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public T TrueValue { get; set; }

        public T FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Object.Equals(value, true) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return Object.Equals(value, TrueValue);
        }
    }

    public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter()
            : base(Visibility.Visible, Visibility.Collapsed)
        {
        }
    }

    public sealed class NegatedBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public NegatedBooleanToVisibilityConverter()
            : base(Visibility.Collapsed, Visibility.Visible)
        {
        }
    }
}
