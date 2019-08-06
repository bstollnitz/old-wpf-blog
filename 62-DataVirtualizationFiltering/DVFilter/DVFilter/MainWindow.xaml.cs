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
using System.Diagnostics;
using DataVirtualization;

namespace DVFilter
{
	public partial class MainWindow : Window
	{
		private CustomerProvider customerProvider;
		private int pageSize = 100;
		private int timePageInMemory = 5000;

		public MainWindow()
		{
			PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;
			this.InitializeComponent();
			this.RefreshData();
		}

		private void CustomerSinceDatePicker_DateChanged(object sender, EventArgs e)
		{
			this.RefreshData();
		}

		private void RefreshData()
		{
			string sortString = "CustomerSince DESC";
			customerProvider = new CustomerProvider(this.CustomerSinceDatePicker.DateFrom, this.CustomerSinceDatePicker.DateTo, sortString);
			AsyncVirtualizingCollection<Customer> customerList = new AsyncVirtualizingCollection<Customer>(customerProvider, pageSize, timePageInMemory);
			this.DataContext = customerList;

			this.CustomersDataGrid.SelectedIndex = 0;
		}
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
