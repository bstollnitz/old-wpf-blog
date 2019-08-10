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
using System.Windows.Data;
using System.ComponentModel;

namespace SortingHierarchy
{
	public partial class SolutionView : UserControl
	{
		private DataSource dataSource;

		public SolutionView()
		{
			this.InitializeComponent();

			this.dataSource = new DataSource();
			CollectionViewSource cvs = (CollectionViewSource)this.Resources["CvsStates"];
			cvs.Source = dataSource.States;
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

			this.dataSource.States[2].Counties.Add(newCounty);
		}
	}

	public class SortCountiesConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			IEnumerable<County> counties = value as IEnumerable<County>;
			CollectionViewSource cvs = new CollectionViewSource();
			cvs.Source = counties;
			cvs.SortDescriptions.Add(new SortDescription("CountyName", ListSortDirection.Ascending));
			return cvs.View;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public class SortCitiesConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			IEnumerable<City> cities = value as IEnumerable<City>;
			CollectionViewSource cvs = new CollectionViewSource();
			cvs.Source = cities;
			cvs.SortDescriptions.Add(new SortDescription("CityName", ListSortDirection.Ascending));
			return cvs.View;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

}
