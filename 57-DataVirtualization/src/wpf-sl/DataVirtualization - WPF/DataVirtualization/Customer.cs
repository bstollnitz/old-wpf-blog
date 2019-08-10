using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataVirtualization
{
    /// <summary>
    /// Demo customer object.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Some dummy data to give the instance a bigger memory footprint.
        /// </summary>
        private byte[] data = new byte[100];
    }
}
