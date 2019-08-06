using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;

namespace DVFilterSort
{
	public static class DataGridHelper
	{
		public static string GetSortMemberPath(DataGridColumn column)
		{
			string sortPropertyName = column.SortMemberPath;
			if (string.IsNullOrEmpty(sortPropertyName))
			{
				DataGridBoundColumn boundColumn = column as DataGridBoundColumn;
				if (boundColumn != null)
				{
					Binding binding = boundColumn.Binding as Binding;
					if (binding != null)
					{
						if (!string.IsNullOrEmpty(binding.XPath))
						{
							sortPropertyName = binding.XPath;
						}
						else if (binding.Path != null)
						{
							sortPropertyName = binding.Path.Path;
						}
					}
				}
			}

			return sortPropertyName;
		}
	}
}
