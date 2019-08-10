using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace PolygonBinding
{
	public class PointCollectionConverter : IValueConverter
	{
		private Dictionary<IEnumerable<Point>, PointCollection> collectionAssociations;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			IEnumerable<Point> enumerable = value as IEnumerable<Point>;
			if (enumerable == null)
			{
				throw new InvalidOperationException("Source collection must be of type IEnumerable<Point>");
			}

			// Construct a dictionary with source and target collection associations if that was not already done.
			if (this.collectionAssociations == null)
			{
				this.collectionAssociations = new Dictionary<IEnumerable<Point>, PointCollection>();
			}

			// If the source is already in the dictionary, return the existing PointCollection
			PointCollection points;
			if (this.collectionAssociations.TryGetValue(enumerable, out points))
			{
				return points;
			}
			else
			{
				// The source is not in the dictionary, so create a new point collection and add it to the dictionary.
				points = new PointCollection(enumerable);
				this.collectionAssociations.Add(enumerable, points);

				// Start listening to collection change events in the new source, if possible.
				INotifyCollectionChanged notifyCollectionChanged = enumerable as INotifyCollectionChanged;
				if (notifyCollectionChanged != null)
				{
					notifyCollectionChanged.CollectionChanged += this.Source_CollectionChanged;
				}

				return points;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException("ConvertBack should never be called");
		}

		private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			IEnumerable<Point> enumerable = sender as IEnumerable<Point>;
			PointCollection points = this.collectionAssociations[enumerable];

			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						points.Insert(e.NewStartingIndex + i, (Point)e.NewItems[i]);
					}
					break;

				case NotifyCollectionChangedAction.Move:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						points.RemoveAt(e.OldStartingIndex);
						points.Insert(e.NewStartingIndex + i, (Point)e.NewItems[i]);
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					for (int i = 0; i < e.OldItems.Count; i++)
					{
						points.RemoveAt(e.OldStartingIndex);
					}
					break;

				case NotifyCollectionChangedAction.Replace:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						points[e.NewStartingIndex + i] = (Point)e.NewItems[i];
					}
					break;

				case NotifyCollectionChangedAction.Reset:
					points.Clear();
					break;
			}
		}
	}
}
