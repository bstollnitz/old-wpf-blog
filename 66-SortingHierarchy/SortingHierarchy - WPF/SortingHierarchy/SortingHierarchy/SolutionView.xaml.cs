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
using System.ComponentModel;

namespace SortingHierarchy
{
	/// <summary>
	/// Interaction logic for SolutionView.xaml
	/// </summary>
	public partial class SolutionView : UserControl
	{
		public SolutionView()
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

	public class SortCountiesConverter2 : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			IEnumerable<County> counties = value as IEnumerable<County>;
			ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(counties);
			lcv.SortDescriptions.Add(new SortDescription("CountyName", ListSortDirection.Ascending));
			return lcv;
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
			ListCollectionView lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(cities);
			lcv.SortDescriptions.Add(new SortDescription("CityName", ListSortDirection.Ascending));
			return lcv;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
