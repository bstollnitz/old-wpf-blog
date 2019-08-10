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

namespace GetListBoxItem
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			InitializeComponent();
		}

		private void ButtonClick(object sender, RoutedEventArgs e)
		{
			GreekGod greekGod = (GreekGod)(listBox.Items[0]);
			ListBoxItem lbi1 = (ListBoxItem)(listBox.ItemContainerGenerator.ContainerFromIndex(0));
			ListBoxItem lbi2 = (ListBoxItem)(listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items.CurrentItem));
		}
	}
}
