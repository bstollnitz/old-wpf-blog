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
using System.IO;
using System.Windows.Threading;
using System.Globalization;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Threading;
using System.Collections;

namespace InitializeDatabase
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void InitializeDatabase_Click(object sender, RoutedEventArgs e)
		{
			this.Status.Text = "Initializing database...";
			this.InitializeDatabaseButton.IsEnabled = false;

			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
			{
				try
				{
					string connectionString = @"Data Source=.\sqlexpress;Initial Catalog=Customers;Integrated Security=True";
					using (Customers db = new Customers(connectionString))
					{
						if (db.DatabaseExists())
						{
							db.DeleteDatabase();
						}
						db.CreateDatabase();

						this.PopulateDatabase(db);
						this.CreateStoredProcedures(db);

						db.SubmitChanges();
						this.UpdateStatusAndEnableButton("Database has been initialized.", this.InitializeDatabaseButton);

						db.Connection.Close();
					}
				}
				catch (Exception exception)
				{
					this.UpdateStatusAndEnableButton("Exception thrown when submitting changes to the database: " + exception.Message, this.InitializeDatabaseButton);
				}
			}));
		}

		private void PopulateDatabase(Customers db)
		{
			List<string> firstNames = this.ReadAllStrings(@"..\..\FirstNames.txt");
			List<string> lastNames = this.ReadAllStrings(@"..\..\LastNames.txt");

			Random random = new Random(5);
			for (int i = 0; i < 10000; i++)
			{
				Customer customer = new Customer
				{
					FirstName = firstNames[random.Next(firstNames.Count)],
					LastName = lastNames[random.Next(lastNames.Count)],
					CustomerSince = DateTime.Today.AddDays(-random.Next(2000)),
					AmountPaidLocalCalls = random.Next(0, 2000),
					AmountPaidNationalCalls = random.Next(0, 3000),
					AmountPaidInternationalCalls = random.Next(0, 500),
					NumberFamilyMembersInPlan = random.Next(0, 3),
					JoinedPreferredProgram = Convert.ToBoolean(random.Next(0, 2)),
					Region = (USRegion)random.Next(0, Enum.GetValues(typeof(USRegion)).Length)
				};
				db.CustomersList.InsertOnSubmit(customer);
			}
		}

		private void CreateStoredProcedures(Customers db)
		{
			string GetSortedFilteredCustomers =
				"CREATE PROCEDURE [dbo].[GetSortedFilteredCustomers] \n" +
				"@StartIndex int, \n" +
				"@EndIndex int, \n" +
				"@BeginDate Datetime, \n" +
				"@EndDate Datetime, \n" +
				"@OrderBy varchar(500) \n" +
				"AS \n" +
				"BEGIN \n" +
				"SET NOCOUNT ON; \n" +
				"DECLARE @SQLStatement varchar(1000) \n" +
				"SET @SQLStatement = \n" +
				"'SELECT Id, FirstName, LastName, CustomerSince, AmountPaidLocalCalls, AmountPaidNationalCalls, AmountPaidInternationalCalls, NumberFamilyMembersInPlan, JoinedPreferredProgram, Region ' + \n" +
				"'FROM Customer cus1 ' + \n" +
				"'JOIN ' + \n" +
				"'(SELECT Id AS Id2, ' + \n" +
				"'ROW_NUMBER() OVER (ORDER BY ' + @OrderBy + ') AS ''RowNumber'' ' + \n" +
				"'FROM Customer ' + \n" +
				"'WHERE CustomerSince BETWEEN ''' + CAST(@BeginDate as varchar(26)) + ''' AND ''' + CAST(@EndDate as varchar(26)) + ''' ) cus2 ' + \n" +
				"'ON cus1.Id = cus2.Id2 ' + \n" +
				"'WHERE cus2.RowNumber BETWEEN ' + CAST(@StartIndex as varchar(10)) + ' AND ' + CAST(@EndIndex as varchar(10)) \n" +
				"PRINT @SQLStatement \n" +
				"EXEC(@SQLStatement) \n" +
				"END ";

			string GetCount =
				"CREATE PROCEDURE [dbo].[GetCount] \n" +
				"@BeginDate Datetime, \n" +
				"@EndDate Datetime \n" +
				"AS \n" +
				"BEGIN \n" +
				"SET NOCOUNT ON; \n" +
				"SELECT COUNT(Id) AS Count \n" +
				"FROM Customer \n" +
				"WHERE CustomerSince BETWEEN @BeginDate AND @EndDate \n" +
				"END ";

			db.ExecuteCommand(GetSortedFilteredCustomers);
			db.ExecuteCommand(GetCount);
		}

		/// <summary>
		/// Clean up consists of:
		/// - Ordering names in the file.
		/// - Removing duplicate names.
		/// - Changing names to have only the first letter capitalized.
		/// </summary>
		private void CleanUp_Click(object sender, RoutedEventArgs e)
		{
			this.Status.Text = "Cleaning up...";
			this.CleanUpButton.IsEnabled = false;

			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
			{
				this.CleanUp(@"..\..\FirstNames.txt");
				this.CleanUp(@"..\..\LastNames.txt");
				this.UpdateStatusAndEnableButton("Files have been cleaned up.", this.CleanUpButton);
			}));
		}

		private void UpdateStatusAndEnableButton(string status, Button btn)
		{
			this.Dispatcher.BeginInvoke(new Action(delegate
			{
				this.Status.Text = status;
				btn.IsEnabled = true;
			}));
		}

		private void CleanUp(string path)
		{
			List<string> duplicatedStrings = this.ReadAllStrings(path);

			using (StreamWriter writer = new StreamWriter(path))
			{
				// Order strings.
				var orderedStrings = duplicatedStrings.OrderBy(s => s);
				// Remove duplicates.
				var uniqueStrings = orderedStrings.Distinct(new CaseInsensitiveComparer());

				writer.Write("");
				foreach (string uniqueString in uniqueStrings)
				{
					// Capitalize first letter.
					string capitalized = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(uniqueString.ToLower());
					writer.WriteLine(capitalized);
				}
			}
		}

		private List<string> ReadAllStrings(string path)
		{
			List<string> stringList = new List<string>();

			using (StreamReader reader = new StreamReader(path))
			{
				string line = reader.ReadLine();
				while (line != null)
				{
					stringList.Add(line);
					line = reader.ReadLine();
				}
			}
			return stringList;
		}

	}

	public class CaseInsensitiveComparer : IEqualityComparer<string>
	{
		public bool Equals(string x, string y)
		{
			return String.Compare(x, y, true) == 0;
		}

		public int GetHashCode(string obj)
		{
			return obj.GetHashCode();
		}
	}
}
