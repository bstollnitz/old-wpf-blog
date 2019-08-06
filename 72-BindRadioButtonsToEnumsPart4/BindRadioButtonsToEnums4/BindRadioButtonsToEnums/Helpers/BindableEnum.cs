using BindRadioButtonsToEnums.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindRadioButtonsToEnums.Helpers
{
    /// <summary>
    /// A wrapper class for an enum value that supports two-way data binding.
    /// </summary>
    /// <typeparam name="TEnum">The enum type.</typeparam>
    public sealed class BindableEnum<TEnum> : BindableBase where TEnum : struct
    {
        /// <summary>
        /// The enum value.
        /// </summary>
        private TEnum enumValue = default(TEnum);

        /// <summary>
        /// Constructs a new bindable enum object.
        /// </summary>
        /// <param name="initialValue">The initial value for the enum.</param>
        public BindableEnum(TEnum initialValue)
        {
            this.enumValue = initialValue;
        }

        /// <summary>
        /// Gets or sets the enum value.
        /// </summary>
        public TEnum Value
        {
            get { return this.enumValue; }
            set
            {
                if (!this.enumValue.Equals(value))
                {
                    this.enumValue = value;
                    this.OnPropertyChanged("Value");
                    this.OnPropertyChanged("Item[]");
                    EventHandler handler = this.ValueChanged;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a Boolean value that is true if and only if the enum value's name
        /// matches the valueName parameter.
        /// </summary>
        /// <param name="valueName">The name of the value to test against.</param>
        /// <returns>True if the enum value's name matches valueName; false, otherwise.</returns>
        /// <remarks>Case is ignored when comparing strings.</remarks>
        public bool this[string valueName]
        {
            get { return this.Value.ToString().Equals(valueName, StringComparison.OrdinalIgnoreCase); }
            set
            {
                TEnum newEnumValue;
                if (value && Enum.TryParse<TEnum>(valueName, true, out newEnumValue))
                {
                    this.Value = newEnumValue;
                }
            }
        }

        /// <summary>
        /// Raised when the Value property changes.
        /// </summary>
        public event EventHandler ValueChanged;
    }
}
