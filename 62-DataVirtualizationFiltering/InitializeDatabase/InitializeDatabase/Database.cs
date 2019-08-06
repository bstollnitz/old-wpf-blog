using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace InitializeDatabase
{
	public class Customers : DataContext
	{
		public Table<Customer> CustomersList;
		public Customers(string connection) : base(connection) { }
	}

	[Table(Name = "Customer")]
	public class Customer
	{
		[Column(IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id;
		[Column]
		public string FirstName;
		[Column]
		public string LastName;
		[Column]
		public DateTime CustomerSince;
		[Column]
		public double AmountPaidLocalCalls;
		[Column]
		public double AmountPaidNationalCalls;
		[Column]
		public double AmountPaidInternationalCalls;
		[Column]
		public int NumberFamilyMembersInPlan;
		[Column]
		public bool JoinedPreferredProgram;
		[Column]
		public USRegion Region;
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
