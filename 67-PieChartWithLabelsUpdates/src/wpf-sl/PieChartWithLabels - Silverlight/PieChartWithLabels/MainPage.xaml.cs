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
using System.Collections.ObjectModel;
using CustomControls;

namespace PieChartWithLabels
{
    public partial class MainPage : UserControl
    {
        private ObservableCollection<City> cities;

        public MainPage()
        {
            this.cities = new ObservableCollection<City>
			{
                new City { Name = "Bellevue", Population = 121347 },
                new City { Name = "Issaquah", Population = 23363 },
                new City { Name = "Redmond", Population = 49427 },
                new City { Name = "Seattle", Population = 602000 },
                new City { Name = "Kirkland", Population = 47325 }
            };
            this.DataContext = this.cities;

            InitializeComponent();
        }

        private int count = 0;
        private Random random = new Random();

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
			int index = random.Next(this.cities.Count + 1);
			this.count++;
			this.cities.Insert(index, new City
			{
				Name = "New City " + this.count,
				Population = random.Next(500000)
			});
		}

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int index = random.Next(this.cities.Count);
            this.cities.RemoveAt(index);
        }


		private void LabeledPieSeries_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.DetailsControl.Content = ((LabeledPieSeries)sender).SelectedItem;
		}
    }

    public class City
    {
        public string Name { get; set; }
        public int Population { get; set; }
    }
}
