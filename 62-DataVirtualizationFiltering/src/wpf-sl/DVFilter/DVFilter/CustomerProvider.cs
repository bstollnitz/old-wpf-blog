using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using DataVirtualization;

namespace DVFilter
{
	public class CustomerProvider : IItemsProvider<Customer>
	{
		private Nullable<DateTime> dateFrom;
		private Nullable<DateTime> dateTo;
		private string sortField;
		private int count;

		public CustomerProvider()
		{
			this.dateFrom = DateTime.Today.AddYears(-100);
			this.dateTo = DateTime.Today.AddYears(100);
			this.sortField = "CustomerSince DESC";
		}

		public CustomerProvider(Nullable<DateTime> dateFrom, Nullable<DateTime> dateTo, string sortField)
			: this()
		{
			if (dateFrom != null)
			{
				this.dateFrom = dateFrom;
			}
			if (dateTo != null)
			{
				this.dateTo = dateTo;
			}

			this.sortField = sortField;
		}

		public int FetchCount()
		{
			CustomersDataContext customersDataContext = new CustomersDataContext();
			count = customersDataContext.GetCount(dateFrom, dateTo).ToList().First().Count.Value;
			return count;
		}

		public IList<Customer> FetchRange(int startIndex, int pageCount, out int overallCount)
		{
			CustomersDataContext customersDataContext = new CustomersDataContext();

			IList<Customer> customersResult;

			startIndex = startIndex + 1; // SQL index starts at 1.
			int endIndex = startIndex + pageCount - 1; // GetCustomers returns items with indices startIndex through endIndex, inclusive.

			overallCount = count; // In this case it's ok not to get the count again because we're assuming the data in the database is not changing.
			customersResult = customersDataContext.GetSortedFilteredCustomers(startIndex, endIndex, this.dateFrom, this.dateTo, this.sortField).ToList();

			return customersResult;
		}
	}
}
