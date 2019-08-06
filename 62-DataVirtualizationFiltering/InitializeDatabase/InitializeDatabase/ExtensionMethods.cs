using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InitializeDatabase
{
	public static class ExtensionMethods
	{
		public static bool ContainsCaseInsensitive(this string bigString, string substring)
		{
			if (String.IsNullOrEmpty(bigString))
			{
				return false;
			}
			else if (String.IsNullOrEmpty(substring))
			{
				return true;
			}
			return (bigString.IndexOf(substring, StringComparison.CurrentCultureIgnoreCase) >= 0);
		}
	}
}
