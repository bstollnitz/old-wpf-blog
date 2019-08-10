using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GetListBoxItem
{
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void ButtonClick(object sender, RoutedEventArgs e)
		{
			GreekGod greekGod = (GreekGod)(listBox.Items[0]);
			ListBoxItem lbi1 = (ListBoxItem)(listBox.ItemContainerGenerator.ContainerFromIndex(0));
		}
	}
}
