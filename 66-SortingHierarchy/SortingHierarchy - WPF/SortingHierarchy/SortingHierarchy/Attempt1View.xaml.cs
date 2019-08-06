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

namespace SortingHierarchy
{
	/// <summary>
	/// Interaction logic for Attempt1View.xaml
	/// </summary>
	public partial class Attempt1View : UserControl
	{
		public Attempt1View()
		{
			this.InitializeComponent();
			this.DataContext = new DataSource();
		}
	}
}
