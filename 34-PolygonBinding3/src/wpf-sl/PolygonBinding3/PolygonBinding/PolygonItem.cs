using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;

namespace PolygonBinding
{
	public class PolygonItem
	{
		private double angleIncrement = Math.PI / Math.Sqrt(2);
		private int initialCount = 249;

		private ObservableCollection<Point> points = new ObservableCollection<Point>();

		public ReadOnlyObservableCollection<Point> Points
		{
			get { return new ReadOnlyObservableCollection<Point>(this.points); }
		}

		public void AddPoint()
		{
			double angle = this.points.Count * this.angleIncrement;
			double x = 250 + 250 * Math.Cos(angle);
			double y = 250 + 250 * Math.Sin(angle);
			this.points.Add(new Point(x, y));
		}

		public PolygonItem()
		{
			while (this.points.Count < this.initialCount)
			{
				this.AddPoint();
			}
		}
	}
}
