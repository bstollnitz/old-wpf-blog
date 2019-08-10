using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

namespace Presenter
{
	public partial class PropertyChangeNotifications
	{
		public PropertyChangeNotifications()
		{
			this.InitializeComponent();
		}

        private void Connect(object sender, RoutedEventArgs e)
        {
            Saturn saturn = this.Resources["saturn"] as Saturn;
            Random random = new Random();
            saturn.Temperature = random.Next(300);
        }
	}
}