# How to sort data virtualized items in WPF

You saw in <a href="..\62-DataVirtualizationFiltering">my last post</a> how you can filter data-virtualized items by delegating the filtering operation to the server. In this post, I will show you how you can sort data-virtualized items on the server by interacting with the DataGrid UI. The code in this post extends the code in the filtering solution from my other post, so make sure you read that first.

As a reminder, the solution I showed in my earlier post exposes a stored procedure called "GetSortedFilteredCustomers" which allows us to sort and filter a subset of customers (we use indices to specify the subset). We can indicate the sorting we want by passing the SQL sorting syntax as a string to the data provider:

	string sortString = "CustomerSince DESC";
	customerProvider = new CustomerProvider(this.CustomerSinceDatePicker.DateFrom, this.CustomerSinceDatePicker.DateTo, sortString);
	AsyncVirtualizingCollection<Customer> customerList = new AsyncVirtualizingCollection<Customer>(customerProvider, pageSize, timePageInMemory);
	this.DataContext = customerList;

In this post I will show how you can construct a sort string that reflects the user's interactions with the DataGrid UI. Here are the behavior requirements for this app:

- Initially, the data is sorted by "CustomerSince", in descending order. To inform the user of this fact, the "CustomerSince" DataGridColumn should display a triangle pointing down.

- If the user clicks on another column, the data should be re-queried to sort based on the clicked column in ascending order. The DataGrid UI should reflect that by displaying an upward pointing triangle in the appropriate column.

- If the user clicks on that column again, the data should now be sorted in descending order.

- If the user presses "Shift" and clicks on several columns, the items should be ordered by all those columns, in the order they were clicked.

With these requirements in mind, I decided that I was going to keep a list of sort descriptions in my app. For each column the user clicks, I want to keep the property name associated with that column, the sort direction (ascending or descending), and a pointer to the actual DataGridColumn. 

	private List<CustomSortDescription> sortDescriptions;
	
	public class CustomSortDescription
	{
		public string PropertyName { get; set; }
		public ListSortDirection Direction { get; set; }
		public DataGridColumn Column { get; set; }
	}

Now we need to be notified whenever the user clicks on a column for sorting. This can easily be done by listening to the "Sorting" DataGrid event.

	<DataGrid x:Name="CustomersDataGrid"
		Sorting="Customers_Sorting"
		...
		>

In the handler for this event, we will add our custom sorting logic. We will also mark it as handled to make sure that the default client-side sorting behavior of DataGrid doesn't kick in.

	private void Customers_Sorting(object sender, DataGridSortingEventArgs e)
	{
		this.ApplySortColumn(e.Column);
		e.Handled = true;
	}

Within ApplySortColumn, we modify the sortDescriptions list to reflect the user's column clicks. If the column clicked was not in the sortDescriptions list we add it, and if it was we flip its direction. If the user is pressing Shift we keep all existing sortDescriptions, and if he's not we remove all except the one the user just clicked on.

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

The GetColumnSortMemberPath method returns the path of the binding associated with a particular DataGridColumn. The code (shown below) relies on the GetSortMemberPath method in DataGridHelper, which was taken from an <a href="http://blogs.msdn.com/vinsibal/archive/2008/08/29/wpf-datagrid-tri-state-sorting-sample.aspx">earlier blog post by Vincent Sibal</a>. Because each of our actual items is held in a DataWrapper instance (for purposes of data virtualization), all customer properties are accessed through the Data property of DataWrapper. Since the SQL server doesn't have data wrappers, we don't want the "Data." portion of the property path when we build the SQL sorting query; we remove it from the path here.

	private string GetColumnSortMemberPath(DataGridColumn column)
	{
		string prefixToRemove = "Data.";
		string fullSortColumn = DataGridHelper.GetSortMemberPath(column);
		string sortColumn = fullSortColumn.Substring(prefixToRemove.Length);
		return sortColumn;
	}

Notice that after changing the sortDescriptions list in the ApplySortColumn method, we call the RefreshData method. RefreshData gets the portion of the SQL query required for sorting based on the sortDescriptions list, and passes that to the CustomerProvider to be executed on the server. Then it updates the page's data context to be the newly created CustomerProvider and ensures that the DataGrid columns display the triangles in each column that reflect the sort descriptions.

	private void RefreshData()
	{
		string sortString = this.GetCurrentSortString();
		customerProvider = new CustomerProvider(this.CustomerSinceDatePicker.DateFrom, this.CustomerSinceDatePicker.DateTo, sortString);
		AsyncVirtualizingCollection<Customer> customerList = new AsyncVirtualizingCollection<Customer>(customerProvider, pageSize, timePageInMemory);
		this.DataContext = customerList;
	
		this.UpdateSortingVisualFeedback();
	
		this.CustomersDataGrid.SelectedIndex = 0;
	}

As you can see below, GetCurrentSortString enumerates through the sortDescriptions list and creates a string with the SQL syntax that reflects that sorting information. For example, if I click on "First Name" twice, and then shift-click on "Local Calls", the string returned by this method would be "FirstName DESC, LocalCalls".

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

Updating the UI to reflect the sortDescriptions list is very easy:

	private void UpdateSortingVisualFeedback()
	{
		foreach (CustomSortDescription csd in this.sortDescriptions)
		{
			csd.Column.SortDirection = csd.Direction;
		}
	}

Finally, we need to think of the initial sorting state of the app. We would like the customers to be displayed in descending order of their "CustomerSince" dates. To make that happen, we can add the following code to the window's constructor:

	public MainWindow()
	{
		...
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

With this code in place, clicking on columns in the DataGrid now causes the corresponding sorting operations to be performed on the server side. This works well in combination with data virtualization. You can see a screenshot of this app below. Notice the triangles in the columns.

<img src="Images/DVFilteringSorting.png" class="postImage" />

If  you're a SQL wizard, I'd love to get your feedback on the stored procedures used in this sample, in particular GetSortedFilteredCustomers. I am by no means a SQL expert and would like to have the current syntax validated or simplified if possible.

