using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace PolygonBinding
{
	[MarkupExtensionReturnType(typeof(PointCollection))]
	public class PointCollectionConnectorExtension : MarkupExtension
	{
		private object source;
		private string propertyName;
		private PointCollection pointCollection;

		public object Source
		{
			get { return this.source; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.source = value;
			}
		}

		public string PropertyName
		{
			get { return this.propertyName; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.propertyName = value;
			}
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			if (this.source == null || this.propertyName == null)
			{
				throw new InvalidOperationException("Cannot use PointCollectionConnector extension without setting Source and PropertyName.");
			}

			// Get the value of the property with name PropertyName from the source object.
			Type sourceType = this.source.GetType();
			PropertyInfo propertyInfo = sourceType.GetProperty(propertyName);
			if (propertyInfo == null)
			{
				throw new InvalidOperationException(String.Format("Source object of type {0} does not have a property named {1}.", sourceType.Name, propertyName));
			}
			object propertyValue = propertyInfo.GetValue(this.source, null);

			// See if the value is an enumerable collection of points.
			IEnumerable<Point> enumerable = propertyValue as IEnumerable<Point>;
			if (enumerable == null)
			{
				throw new InvalidOperationException(String.Format("Source object of type {0} has a property named {1}, but its value  (of type {2}) doesn't implement IEnumerable<Point>.", sourceType.Name, propertyName, propertyValue.GetType().Name));
			}

			// Construct the initial point collection by copying points from the enumerable collection.
			this.pointCollection = new PointCollection(enumerable);

			// Listen for collection changed events coming from the source, if possible.
			INotifyCollectionChanged notifyCollectionChanged = propertyValue as INotifyCollectionChanged;
			if (notifyCollectionChanged != null)
			{
				notifyCollectionChanged.CollectionChanged += this.Source_CollectionChanged;
			}

			return this.pointCollection;
		}

		private void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						this.pointCollection.Insert(e.NewStartingIndex + i, (Point)e.NewItems[i]);
					}
					break;

				case NotifyCollectionChangedAction.Move:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						this.pointCollection.RemoveAt(e.OldStartingIndex);
						this.pointCollection.Insert(e.NewStartingIndex + i, (Point)e.NewItems[i]);
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					for (int i = 0; i < e.OldItems.Count; i++)
					{
						this.pointCollection.RemoveAt(e.OldStartingIndex);
					}
					break;

				case NotifyCollectionChangedAction.Replace:
					for (int i = 0; i < e.NewItems.Count; i++)
					{
						this.pointCollection[e.NewStartingIndex + i] = (Point)e.NewItems[i];
					}
					break;

				case NotifyCollectionChangedAction.Reset:
					this.pointCollection.Clear();
					break;
			}
		}
	}
}
