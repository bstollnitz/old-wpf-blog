using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using DataVirtualization;

namespace DVFilterSort
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private CustomerProvider customerProvider;
		private int pageSize = 100;
		private int timePageInMemory = 5000;
		private List<CustomSortDescription> sortDescriptions;

		public MainWindow()
		{
			PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;

			InitializeComponent();

			string defaultSortColumnName = "CustomerSince";
			DataGridColumn defaultSortColumn = this.CustomersDataGrid.Columns.Single(dgc => this.GetColumnSortMemberPath(dgc) == defaultSortColumnName);
			this.sortDescriptions = new List<CustomSortDescription>
			{
				new CustomSortDescription
				{
					PropertyName = defaultSortColumnName,
					Direction = ListSortDirection.Descending,
					Column = defaultSortColumn
				}
			};
			this.RefreshData();

		}

		private void CustomerSinceDatePicker_DateChanged(object sender, EventArgs e)
		{
			this.RefreshData();
		}

		private void Customers_Sorting(object sender, DataGridSortingEventArgs e)
		{
			this.ApplySortColumn(e.Column);
			e.Handled = true;
		}

		private void RefreshData()
		{
			string sortString = this.GetCurrentSortString();
			customerProvider = new CustomerProvider(this.CustomerSinceDatePicker.DateFrom, this.CustomerSinceDatePicker.DateTo, sortString);
			AsyncVirtualizingCollection<Customer> customerList = new AsyncVirtualizingCollection<Customer>(customerProvider, pageSize, timePageInMemory);
			this.DataContext = customerList;

			this.UpdateSortingVisualFeedback();

			this.CustomersDataGrid.SelectedIndex = 0;
		}

		private void ApplySortColumn(DataGridColumn column)
		{
			// If column was not sorted, we sort it ascending. If it was already sorted, we flip the sort direction.
			string sortColumn = this.GetColumnSortMemberPath(column);
			CustomSortDescription existingSortDescription = this.sortDescriptions.SingleOrDefault(sd => sd.PropertyName == sortColumn);
			if (existingSortDescription == null)
			{
				existingSortDescription = new CustomSortDescription
				{
					PropertyName = sortColumn,
					Direction = ListSortDirection.Ascending,
					Column = column
				};
				this.sortDescriptions.Add(existingSortDescription);
			}
			else
			{
				existingSortDescription.Direction = (existingSortDescription.Direction == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
			}

			// If user is not pressing Shift, we remove all SortDescriptions except the current one.
			bool isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
			if (!isShiftPressed)
			{
				for (int i = this.sortDescriptions.Count - 1; i >= 0; i--)
				{
					CustomSortDescription csd = this.sortDescriptions[i];
					if (csd.PropertyName != sortColumn)
					{
						this.sortDescriptions.RemoveAt(i);
					}
				}
			}

			this.RefreshData();
		}

		private string GetColumnSortMemberPath(DataGridColumn column)
		{
			string prefixToRemove = "Data.";
			string fullSortColumn = DataGridHelper.GetSortMemberPath(column);
			string sortColumn = fullSortColumn.Substring(prefixToRemove.Length);
			return sortColumn;
		}

		private string GetCurrentSortString()
		{
			// The result string is created, taking into account all sorted columns in the order they were sorted.
			StringBuilder result = new StringBuilder();
			string separator = String.Empty;
			foreach (CustomSortDescription sd in this.sortDescriptions)
			{
				result.Append(separator);
				result.Append(sd.PropertyName);
				if (sd.Direction == ListSortDirection.Descending)
				{
					result = result.Append(" DESC");
				}
				separator = ", ";
			}

			return result.ToString();
		}

		private void UpdateSortingVisualFeedback()
		{
			foreach (CustomSortDescription csd in this.sortDescriptions)
			{
				csd.Column.SortDirection = csd.Direction;
			}
		}

	}

	public class CustomSortDescription
	{
		public string PropertyName { get; set; }
		public ListSortDirection Direction { get; set; }
		public DataGridColumn Column { get; set; }
	}

	public enum USRegion
	{
		Northeast,
		Southeast,
		Midwest,
		Southwest,
		West
	}
}
