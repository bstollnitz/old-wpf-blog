using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace PolygonBinding
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{

		public Window1()
		{
			InitializeComponent();
		}

		private void ChangeSource(object sender, RoutedEventArgs e)
		{
			PolygonItem polygon = this.Resources["src"] as PolygonItem;
			polygon.Subdivide();
		}
	}

	public class PolygonItem : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private List<Point> points = new List<Point>();

		public ReadOnlyCollection<Point> Points
		{
			get { return new ReadOnlyCollection<Point>(points); }
			set
			{
				points = new List<Point>(value);
				OnPropertyChanged("Points");
			}
		}

		public void Subdivide()
		{
			int count = this.points.Count;
			List<Point> newPoints = new List<Point>(count * 2);
			for (int i = 0; i < count; i++)
			{
				Point previousPoint = this.points[i];
				Point nextPoint = this.points[(i + 1) % count];

				newPoints.Add(previousPoint);
				Vector offset = nextPoint - previousPoint;
				Vector normal = new Vector(offset.Y, -offset.X);
				newPoints.Add(previousPoint + 0.5 * offset + 0.4 * normal);
			}
			this.points = newPoints;
			OnPropertyChanged("Points");
		}

		public PolygonItem()
		{
			points.Add(new Point(275, 100));
			points.Add(new Point(375, 200));
			points.Add(new Point(275, 300));
			points.Add(new Point(175, 200));
		}
	}

	public class Converter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			IEnumerable<Point> enumerable = value as IEnumerable<Point>;
			PointCollection points = null;
			if (enumerable != null)
			{
				points = new PointCollection(enumerable);
			}
			return points;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException("ConvertBack should never be called");
		}
	}
}