using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GetListBoxItem
{
	public class GreekGod
	{
		private string name;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		private string description;

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		private string romanName;

		public string RomanName
		{
			get { return romanName; }
			set { romanName = value; }
		}

		public GreekGod(string name, string description, string romanName)
		{
			this.name = name;
			this.description = description;
			this.romanName = romanName;
		}
	}
}