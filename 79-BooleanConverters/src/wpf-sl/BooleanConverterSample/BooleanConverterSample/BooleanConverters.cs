using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ZagStudio.Converters
{
	/// <summary>
	/// Abstract base class for value converters that convert between Boolean
	/// values and other types.
	/// </summary>
	public abstract class BooleanConverter<T> : IValueConverter
	{
		protected BooleanConverter(T trueValue, T falseValue)
		{
			this.TrueValue = trueValue;
			this.FalseValue = falseValue;
		}

		public T TrueValue { get; set; }

		public T FalseValue { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return Object.Equals(value, true) ? this.TrueValue : this.FalseValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return Object.Equals(value, this.TrueValue) ? true : false;
		}
	}

	/// <summary>
	/// Converts true to false and false to true.
	/// </summary>
	public sealed class NegatedBooleanConverter : BooleanConverter<bool>
	{
		public NegatedBooleanConverter()
			: base(false, true)
		{
		}
	}

	/// <summary>
	/// Converts true and false to two different string values. By default,
	/// both true and false are converted to null.
	/// </summary>
	public sealed class BooleanToStringConverter : BooleanConverter<string>
	{
		public BooleanToStringConverter()
			: base(null, null)
		{
		}
	}

	/// <summary>
	/// Converts Boolean values to Visibility values. By default, true is
	/// converted to Visible and false is converted to Collapsed.
	/// </summary>
	public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
	{
		public BooleanToVisibilityConverter()
			: base(Visibility.Visible, Visibility.Collapsed)
		{
		}
	}

	/// <summary>
	/// Converts Boolean values to double values. By default, true is
	/// converted to 1 and false is converted to 0.
	/// </summary>
	public sealed class BooleanToDoubleConverter : BooleanConverter<double>
	{
		public BooleanToDoubleConverter()
			: base(1, 0)
		{
		}
	}

	/// <summary>
	/// Converts Boolean values to Brush values. By default, both
	/// true and false are converted to null.
	/// </summary>
	public sealed class BooleanToBrushConverter : BooleanConverter<Brush>
	{
		public BooleanToBrushConverter()
			: base(null, null)
		{
		}
	}
}
