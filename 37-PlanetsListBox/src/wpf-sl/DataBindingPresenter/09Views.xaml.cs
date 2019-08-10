using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.ComponentModel;
using System.Globalization;

namespace Presenter
{
	public partial class Views
	{
        SolarSystem solarSystem;
        ListCollectionView view;

        public Views()
		{
			this.InitializeComponent();
            solarSystem = this.Resources["solarSystem"] as SolarSystem;
            view = CollectionViewSource.GetDefaultView(solarSystem.SolarSystemObjects) as ListCollectionView;
        }

        private void AddSorting(object sender, RoutedEventArgs e)
        {
            view.SortDescriptions.Add(new SortDescription("Diameter", 
                ListSortDirection.Ascending));
        }

        private void RemoveSorting(object sender, RoutedEventArgs e)
        {
            view.SortDescriptions.Clear();
        }

        private void AddFiltering(object sender, RoutedEventArgs e)
        {
            view.Filter = new Predicate<object>(FilterOutSmallPlanets);
        }

        private bool FilterOutSmallPlanets(object item)
        {
            SolarSystemObject solarSystemObject = item as SolarSystemObject;
            if (solarSystemObject.Diameter < 70000)
            {
                return false;
            }
            return true;
        }

        private void RemoveFiltering(object sender, RoutedEventArgs e)
        {
            view.Filter = null;
        }

        private void AddGrouping(object sender, RoutedEventArgs e)
        {
            view.GroupDescriptions.Add(
                new PropertyGroupDescription("Diameter", 
                new GroupConverter()));
        }

        private void RemoveGrouping(object sender, RoutedEventArgs e)
        {
            view.GroupDescriptions.Clear();
        }
    }

    public class GroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double diameter = (double)value;
            if (diameter < 70000)
            {
                return "Small";
            }
            return "Large";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("This method should never be called.");
        }
    }
}