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
	public partial class BindToCollections
	{
        public BindToCollections()
		{
			this.InitializeComponent();
        }

        private void AddPlanetX(object sender, RoutedEventArgs e)
        {
            SolarSystem system = this.Resources["solarSystem"] as SolarSystem;
            system.SolarSystemObjects.Add(new SolarSystemObject("Planet X", 3, 50000, new Uri(@"Images\planetx.jpg", UriKind.Relative), "This planet inhabited by aliens is cloaked so that humans can't see it."));
        }  
	}
}