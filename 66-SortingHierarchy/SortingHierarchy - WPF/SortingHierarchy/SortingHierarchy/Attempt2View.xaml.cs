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
	/// Interaction logic for Attempt2View.xaml
	/// </summary>
	public partial class Attempt2View : UserControl
	{
		public Attempt2View()
		{
			this.InitializeComponent();
			this.DataContext = new DataSource();
		}
	}
}
