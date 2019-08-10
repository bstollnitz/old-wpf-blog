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
	/// Interaction logic for Attempt4View.xaml
	/// </summary>
	public partial class Attempt4View : UserControl
	{
		public Attempt4View()
		{
			this.InitializeComponent();
			this.DataContext = new DataSource();
		}

		private void AddItem_Click(object sender, RoutedEventArgs e)
		{
			County newCounty = new County
			{
				CountyName = "New County",
				Cities =
				{
					new City { CityName = "Metropolis" },
					new City { CityName = "Big Town" },
					new City { CityName = "Cityville" }
				}
			};
			DataSource dataSource = (DataSource)this.DataContext;
			dataSource.States[2].Counties.Add(newCounty);
		}
	}

	public class SortCountiesConverter1 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			IEnumerable<County> counties = value as IEnumerable<County>;
			if (counties != null)
			{
				return counties.OrderBy(county => county.CountyName);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class SortCitiesConverter1 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			IEnumerable<City> cities = value as IEnumerable<City>;
			if (cities != null)
			{
				return cities.OrderBy(city => city.CityName);
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
