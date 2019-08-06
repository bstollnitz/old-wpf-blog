using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace BindRadioButtonsToEnums.Converters
{
    public class EnumToBooleanConverter<TEnum> : IValueConverter where TEnum : struct
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Convert parameter from string to enum if needed.
            TEnum enumValue;
            if (parameter is string && Enum.TryParse<TEnum>((string)parameter, true, out enumValue))
            {
                parameter = enumValue;
            }

            // Return true if value matches parameter.
            return Object.Equals(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // If value is true, then return the enum value corresponding to parameter.
            if (Object.Equals(value, true))
            {
                // Convert parameter from string to enum if needed.
                TEnum enumValue;
                if (parameter is string && Enum.TryParse<TEnum>((string)parameter, true, out enumValue))
                {
                    parameter = enumValue;
                }
                return parameter;
            }

            // Otherwise, return UnsetValue, which is ignored by bindings.
            return DependencyProperty.UnsetValue;
        }
    }
}
