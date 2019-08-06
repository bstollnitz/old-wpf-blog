using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.OleDb;
using System.Data;
using System.ComponentModel;
using System.Collections;


namespace ThreeLevelMasterDetailADO
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : Window
	{
		DataSet ds;

		public Window1()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			ds = GetData();
			sp.DataContext = ds.Tables["Mountains"];
		}

		private void AddItem(object sender, RoutedEventArgs args)
		{
			DataRow row = ds.Tables["Mountains"].NewRow();
			row["Mountain_ID"] = 4;
			row["Mountain_Name"] = "Big White";
			ds.Tables["Mountains"].Rows.Add(row);
			add.IsEnabled = false;
		}

		private void FilterItems(object sender, RoutedEventArgs args)
		{
			BindingListCollectionView view = (BindingListCollectionView)CollectionViewSource.GetDefaultView(ds.Tables["Mountains"]);
			//bool canFilter = view.CanFilter; // false
			//bool canCustomFilter = view.CanCustomFilter; // true
			
			view.CustomFilter = "Mountain_Name <> 'Crystal Mountain'";
		}

		private DataSet GetData()
		{
			string mdbFile = "SkiMountains.mdb";
			string connString =
				string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0}", mdbFile);
			OleDbConnection conn = new OleDbConnection(connString);

			DataSet dataSet = new DataSet();

			OleDbDataAdapter mountainsAdapter = new OleDbDataAdapter();
			mountainsAdapter.SelectCommand = new OleDbCommand("SELECT * FROM Mountains;", conn);
			mountainsAdapter.Fill(dataSet, "Mountains");

			OleDbDataAdapter liftsAdapter = new OleDbDataAdapter();
			liftsAdapter.SelectCommand = new OleDbCommand("SELECT * FROM Lifts;", conn);
			liftsAdapter.Fill(dataSet, "Lifts");

			OleDbDataAdapter runsAdapter = new OleDbDataAdapter();
			runsAdapter.SelectCommand = new OleDbCommand("SELECT * FROM Runs;", conn);
			runsAdapter.Fill(dataSet, "Runs");

			DataTableCollection tables = dataSet.Tables;
			dataSet.Relations.Add("MountainsLifts",
				tables["Mountains"].Columns["Mountain_ID"],
				tables["Lifts"].Columns["Mountain_ID"]);

			dataSet.Relations.Add("LiftsRuns",
				tables["Lifts"].Columns["Lift_ID"],
				tables["Runs"].Columns["Lift_ID"]);

			return dataSet;
		}
	}
}