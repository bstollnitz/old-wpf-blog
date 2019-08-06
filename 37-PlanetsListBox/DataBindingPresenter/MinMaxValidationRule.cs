using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Presenter
{
    class MinMaxValidationRule : ValidationRule
    {
        private double minimum;

        public double Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        private double maximum;

        public double Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }
	

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            double valueTyped;
            bool isDouble = Double.TryParse((string)value, out valueTyped);
            if (isDouble && (minimum <= valueTyped) && 
                (valueTyped <= maximum))
            {
                return new ValidationResult(true, null);
            }
            return new ValidationResult(false, "The value typed should be between " + minimum + " and " + maximum + ".");
        }
    }
}
