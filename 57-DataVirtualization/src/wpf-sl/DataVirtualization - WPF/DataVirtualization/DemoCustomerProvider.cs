using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace DataVirtualization
{
    /// <summary>
    /// Demo implementation of IItemsProvider returning dummy customer items after
    /// a pause to simulate network/disk latency.
    /// </summary>
    public class DemoCustomerProvider : IItemsProvider<Customer>
    {
        private int _count;
        private readonly int _fetchDelay;

        /// <summary>
        /// Initializes a new instance of the <see cref="DemoCustomerProvider"/> class.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="fetchDelay">The fetch delay.</param>
        public DemoCustomerProvider(int count, int fetchDelay)
        {
            _count = count;
            _fetchDelay = fetchDelay;
        }

        /// <summary>
        /// Fetches the total number of items available.
        /// </summary>
        /// <returns></returns>
        public int FetchCount()
        {
            Trace.WriteLine("FetchCount");
            Thread.Sleep(_fetchDelay);
            return _count;
        }

        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns></returns>
        public IList<Customer> FetchRange(int startIndex, int pageCount, out int overallCount)
        {
            Trace.WriteLine("FetchRange: " + startIndex + "," + pageCount);
            Thread.Sleep(_fetchDelay);

            overallCount = _count;
            List<Customer> list = new List<Customer>();
            for (int i = startIndex; i < startIndex + pageCount && i < this._count; i++)
            {
                Customer customer = new Customer { Id = i + 1, Name = "Customer " + (i + 1) };
                list.Add(customer);
            }
            return list;
        }

        /// <summary>
        /// Pretend to insert an item.
        /// </summary>
        public void InsertItem()
        {
            _count++;
        }

        /// <summary>
        /// Pretend to remove an item.
        /// </summary>
        public void RemoveItem()
        {
            _count--;
        }
    }
}
