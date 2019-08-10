using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows;

namespace MultipleStyles
{
	[MarkupExtensionReturnType(typeof(Style))]
	public class MultiStyleExtension : MarkupExtension
	{
		private string[] resourceKeys;

		/// <summary>
		/// Public constructor.
		/// </summary>
		/// <param name="inputResourceKeys">The constructor input should be a string consisting of one or more style names separated by spaces.</param>
		public MultiStyleExtension(string inputResourceKeys)
		{
			if (inputResourceKeys == null)
			{
				throw new ArgumentNullException("inputResourceKeys");
			}

			this.resourceKeys = inputResourceKeys.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			if (this.resourceKeys.Length == 0)
			{
				throw new ArgumentException("No input resource keys specified.");
			}
		}

		/// <summary>
		/// Returns a style that merges all styles with the keys specified in the constructor.
		/// </summary>
		/// <param name="serviceProvider">The service provider for this markup extension.</param>
		/// <returns>A style that merges all styles with the keys specified in the constructor.</returns>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			Style resultStyle = new Style();

			foreach (string currentResourceKey in resourceKeys)
			{
				Style currentStyle = new StaticResourceExtension(currentResourceKey).ProvideValue(serviceProvider) as Style;

				if (currentStyle == null)
				{
					throw new InvalidOperationException("Could not find style with resource key " + currentResourceKey + ".");
				}

				resultStyle.Merge(currentStyle);
			}
			return resultStyle;
		}
	}
}
