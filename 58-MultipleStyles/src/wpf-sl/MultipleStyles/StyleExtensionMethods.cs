using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MultipleStyles
{
	public static class StyleExtensionMethods
	{
		/// <summary>
		/// Merges the two styles passed as parameters. The first style will be modified to include any 
		/// information present in the second. If there are collisions, the second style takes priority.
		/// </summary>
		/// <param name="style1">First style to merge, which will be modified to include information from the second one.</param>
		/// <param name="style2">Second style to merge.</param>
		public static void Merge(this Style style1, Style style2)
		{
			if (style1 == null)
			{
				throw new ArgumentNullException("style1");
			}
			if (style2 == null)
			{
				throw new ArgumentNullException("style2");
			}

			if (style1.TargetType.IsAssignableFrom(style2.TargetType))
			{
				style1.TargetType = style2.TargetType;
			}

			if (style2.BasedOn != null)
			{
				Merge(style1, style2.BasedOn);
			}

			foreach (SetterBase currentSetter in style2.Setters)
			{
				style1.Setters.Add(currentSetter);
			}

			foreach (TriggerBase currentTrigger in style2.Triggers)
			{
				style1.Triggers.Add(currentTrigger);
			}

			// This code is only needed when using DynamicResources.
			foreach (object key in style2.Resources.Keys)
			{
				style1.Resources[key] = style2.Resources[key];
			}
		}
	}
}
