# How to data bind a Polygon’s Points to a data source – Part III

TechEd Barcelona is coming up next week, between Tuesday the 7th and Friday the 10th. Some members of the Avalon team will be there, and I'm very fortunate to be part of that group. If you read my blog and are attending this conference, come by and introduce yourself (don't be shy!!). I will spend most of my time helping with the Avalon labs, but you may also find me in the Avalon booth. I can't wait to meet some of you, hear about the applications you've been developing with Avalon, brainstorm with you about your data binding scenarios, and hear all the feedback (good and bad) you have about this platform. 

![](Images/TechEdBarcelona.GIF)

In my <a href="..\33-PolygonBinding2">last post</a>, I talked about a way to bind a Polygon's Points to a data source that had the following advantages:
- Changes in the source are propagated to the UI.
- There is a clean separation between the UI and data layers.
- It scales well for scenarios where you're making small frequent changes to the source collection.

However, this solution had one drawback: it can not be used in Styles. Today I will show you a third solution to the same problem with all the advantages above, plus it can be used in Styles. I will be using the same data source I used in my previous post.

This time I decided to use a Converter. The code in the Convert method is very similar to the code in the ProvideValue method of the MarkupExtension from my previous post. In both implementations, we need to return the PointCollection that the Polygon's Points property will be set to. Also, in both scenarios, we need to hook up an event handler to listen for collection changes in the source and replicate those changes in the PointCollection. 

There are some differences, too. Of course, this time we don't have to use reflection to get the source collection, since that gets passed as the first parameter of the Convert method. Another difference is that it is possible that one instance of the Converter will be used by several Bindings, which requires a little bit of coordination on our part. Here is the complete implementation of the Converter:

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

You probably noticed that the main difference between this code and the one in the MarkupExtension solution is the Dictionary. This is the coordination bit I talked about earlier. Let's imagine for a while that, instead of this dictionary, we have two private members that hold the source collection and the PointCollection, just like in the MarkupExtension solution. Now imagine we have two Bindings that use this same Converter with different source collections. Here are the results of a common sequence of events:

- The source collection of the second Binding changes. The private variables hold the source collection and PointCollection for the second Binding.

- Now the source collection of the first Binding changes. Remember that the same event handler is used to handle changes to both source collections. The event handler is called, but makes the changes to the second PointCollection because that's what the private variable holds.

As a general rule, holding state in a Converter is bad practice because it can cause trouble when shared. 

I solved this problem by introducing a Dictionary that keeps an association between a source collection and the corresponding PointCollection. This way, the collection change handler is able to retrieve the PointCollection it needs to change at any point in time. Also, notice that if the same instance of the Converter is used twice with the same source collection, the second time it is used we return the PointCollection we already have. 

Here is the XAML used in this solution:

	<Window.Resources>
		<local:PolygonItem x:Key="src"/>
		<local:PointCollectionConverter x:Key="converter"/>
	</Window.Resources>
	
	<StackPanel>
		<Button Click="ChangeSource" Margin="10" HorizontalAlignment="Center">Change data source</Button>
		<Polygon Name="polygonElement" Width="500" Height="500" Margin="25" Fill="#CD5C5C" Points="{Binding Source={StaticResource src}, Path=Points, Converter={StaticResource converter}}"/>
	</StackPanel>

The event handler for the Button is the same as in my previous post, so I won't show it again.

Once again, the solution I explained sounds pretty good. So, what is the drawback this time?  The one drawback I can think of is that we're still holding state in the Converter - we are keeping an instance of the Dictionary. Sure, we are holding state so we don't get in trouble by holding state some other way. In general, I would like to discourage people from keeping state in a Converter. In this case we put quite a bit of thought into the state we're keeping around, so it's not all that bad, but please use this technique with reservations.

I won't bother showing a screenshot here, since the application for this post looks identical to the one in my previous post.
