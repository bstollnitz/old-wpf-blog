using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.OleDb;
using System.ComponentModel;

namespace ADOIndependentView
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{
		CollectionViewSource cvs1;
		CollectionViewSource cvs3;

		public Window1()
		{
			this.Resources.Add("sacredRiversTable", this.GetData());
			InitializeComponent();

			cvs1 = this.Resources["cvs1"] as CollectionViewSource;
			cvs3 = this.Resources["cvs3"] as CollectionViewSource;
		}

		private void SortCvs1DescendingHandler(object sender, RoutedEventArgs args)
		{
			cvs1.SortDescriptions.Clear();
			cvs1.SortDescriptions.Add(new SortDescription("RiverName", ListSortDirection.Descending));
			((Button)sender).IsEnabled = false;
		}

		private void SortCvs3DescendingHandler(object sender, RoutedEventArgs args)
		{
			cvs3.SortDescriptions.Clear();
			cvs3.SortDescriptions.Add(new SortDescription("RiverName", ListSortDirection.Descending));
			((Button)sender).IsEnabled = false;
		}

		private DataTable GetData()
		{
			string mdbFile = "IndiaSacredRivers.mdb";
			string connString =
				string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}", mdbFile);
			OleDbConnection conn = new OleDbConnection(connString);

			DataTable sacredRiversTable = new DataTable();

			OleDbDataAdapter sacredRiversAdapter = new OleDbDataAdapter();
			sacredRiversAdapter.SelectCommand = new OleDbCommand("SELECT * FROM SacredRivers;", conn);
			sacredRiversAdapter.Fill(sacredRiversTable);

			return sacredRiversTable;
		}
	}
}