using System;
using System.Collections.Generic;
using System.Text;

namespace DataBoundDialogBox
{
    public class DataSource
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string comment;

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
    }
}
