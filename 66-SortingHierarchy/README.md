# How to sort a hierarchy

Today's blog post discusses how you can sort data items at each level of a hierarchical user interface in WPF and Silverlight. Instead of just giving you the final solution, I will first show a few approaches that you might expect to work, and I'll explain why they don't work.  All of the approaches will use a single data source: a list of State objects, each of which contains a list of County objects, each of which in turn contains a list of City objects.  I'll assume you're familiar with the sorting abilities of CollectionViewSource (see <a href="http://www.zagstudio.com/blog/387">this post</a>) and binding to hierarchical data (see <a href="http://www.zagstudio.com/blog/356">this post</a>).

## WPF

### Attempt 1

The first approach most people try is to add a CollectionViewSource (with a SortDescription) for each level of the hierarchy directly to the root element's resources, and then bind the ItemsSource property of each HierarchicalDataTemplate to the corresponding CollectionViewSource. The XAML below shows the resources, assuming the root element's DataContext has a property called "States" that returns a collection of State objects.

	<CollectionViewSource Source="{Binding Path=Cities}" x:Key="CvsCities">
		<CollectionViewSource.SortDescriptions>
			<scm:SortDescription PropertyName="CityName"/>
		</CollectionViewSource.SortDescriptions>
	</CollectionViewSource>
	
	<CollectionViewSource Source="{Binding Path=Counties}" x:Key="CvsCounties">
		<CollectionViewSource.SortDescriptions>
			<scm:SortDescription PropertyName="CountyName" />
		</CollectionViewSource.SortDescriptions>
	</CollectionViewSource>
	
	<CollectionViewSource Source="{Binding Path=States}" x:Key="CvsStates">
		<CollectionViewSource.SortDescriptions>
			<scm:SortDescription PropertyName="StateName" />
		</CollectionViewSource.SortDescriptions>
	</CollectionViewSource>
	
	<DataTemplate x:Key="CityTemplate">
		<TextBlock Text="{Binding Path=CityName}" />
	</DataTemplate>
	
	<HierarchicalDataTemplate x:Key="CountyTemplate" DataType="{x:Type local:County}" ItemsSource="{Binding Source={StaticResource CvsCities}}" ItemTemplate="{StaticResource CityTemplate}">
		<TextBlock Text="{Binding Path=CountyName}" />
	</HierarchicalDataTemplate>
	
	<HierarchicalDataTemplate x:Key="StateTemplate" DataType="{x:Type local:State}" ItemsSource="{Binding Source={StaticResource CvsCounties}}" ItemTemplate="{StaticResource CountyTemplate}">
		<TextBlock Text="{Binding Path=StateName}" />
	</HierarchicalDataTemplate>

This approach doesn't work -- the names of the states show up, but without any counties or cities.  That's because all three CollectionViewSources have the same DataContext: the data source with the States collection. The DataContext is not determined based on where a CollectionViewSource is consumed, as you might expect -- it depends only on where the CollectionViewSource is declared. Visual Studio's output window shows two binding errors indicating that there are no Cities or Counties properties:

System.Windows.Data Error: 39 : BindingExpression path error: 'Cities' property not found on 'object' ''DataSource'...
System.Windows.Data Error: 39 : BindingExpression path error: 'Counties' property not found on 'object' ''DataSource'...

### Attempt 2

With this in mind, it's natural to look for a solution where the CollectionViewSources are added directly to the HierarchicalDataTemplates, so that each one has a DataContext that is set to the appropriate data item. This approach adds a CollectionViewSource to the Resources section of each HierarchicalDataTemplate:

	<CollectionViewSource Source="{Binding States}" x:Key="CvsStates">
		<CollectionViewSource.SortDescriptions>
			<scm:SortDescription PropertyName="StateName" />
		</CollectionViewSource.SortDescriptions>
	</CollectionViewSource>
	
	<DataTemplate x:Key="CityTemplate">
		<TextBlock Text="{Binding CityName}" />
	</DataTemplate>
	
	<HierarchicalDataTemplate x:Key="CountyTemplate" DataType="{x:Type local:County}" ItemTemplate="{StaticResource CityTemplate}">
		<HierarchicalDataTemplate.Resources>
			<CollectionViewSource Source="{Binding Path=Cities}" x:Key="CvsCities">
				<CollectionViewSource.SortDescriptions>
					<scm:SortDescription PropertyName="CityName"/>
				</CollectionViewSource.SortDescriptions>
			</CollectionViewSource>
		</HierarchicalDataTemplate.Resources>
		<HierarchicalDataTemplate.ItemsSource>
			<Binding Source="{StaticResource CvsCities}" />
		</HierarchicalDataTemplate.ItemsSource>
		
		<TextBlock Text="{Binding CountyName}" />
	</HierarchicalDataTemplate>
	
	<HierarchicalDataTemplate x:Key="StateTemplate" DataType="{x:Type local:State}" ItemTemplate="{StaticResource CountyTemplate}">
		<HierarchicalDataTemplate.Resources>
			<CollectionViewSource Source="{Binding Path=Counties}" x:Key="CvsCounties">
				<CollectionViewSource.SortDescriptions>
					<scm:SortDescription PropertyName="CountyName" />
				</CollectionViewSource.SortDescriptions>
			</CollectionViewSource>
		</HierarchicalDataTemplate.Resources>
		<HierarchicalDataTemplate.ItemsSource>
			<Binding Source="{StaticResource CvsCounties}" />
		</HierarchicalDataTemplate.ItemsSource>
		
		<TextBlock Text="{Binding StateName}" />
	</HierarchicalDataTemplate>

This also doesn't work. Again, the states appear but the counties and cities don't.  Although each instance of the HierarchicalDataTemplate has its DataContext set to the appropriate data item, the resources section does not share that same DataContext. This happens because the template's resources are shared among all template instances (and each template instance has a different DataContext).  The output window in Visual Studio shows two error messages indicating that the Cities and Counties bindings are not inheriting any DataContext:

System.Windows.Data Error: 2 : Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression:Path=Cities; DataItem=null...
System.Windows.Data Error: 2 : Cannot find governing FrameworkElement or FrameworkContentElement for target element. BindingExpression:Path=Counties; DataItem=null...

### Attempt 3

Resources within the HierarchicalDataTemplates don't have the correct DataContext, so we need to find a way to add each CollectionViewSource to a property of the appropriate HierarchicalDataTemplate. We can't set the HierarchicalDataTemplate's ItemsSource property directly to the CollectionViewSource because the CollectionViewSource does not implement IEnumerable , but we can use a binding:

	<CollectionViewSource Source="{Binding States}" x:Key="CvsStates">
		<CollectionViewSource.SortDescriptions>
			<scm:SortDescription PropertyName="StateName" />
		</CollectionViewSource.SortDescriptions>
	</CollectionViewSource>
	
	<DataTemplate x:Key="CityTemplate">
		<TextBlock Text="{Binding CityName}" />
	</DataTemplate>
	
	<HierarchicalDataTemplate x:Key="CountyTemplate" DataType="{x:Type local:County}" ItemTemplate="{StaticResource CityTemplate}">
		<HierarchicalDataTemplate.ItemsSource>
			<Binding>
				<Binding.Source>
					<CollectionViewSource Source="{Binding Path=Cities}">
						<CollectionViewSource.SortDescriptions>
							<scm:SortDescription PropertyName="CityName"/>
						</CollectionViewSource.SortDescriptions>
					</CollectionViewSource>
				</Binding.Source>
			</Binding>
		</HierarchicalDataTemplate.ItemsSource>
		
		<TextBlock Text="{Binding CountyName}" />
	</HierarchicalDataTemplate>
	
	<HierarchicalDataTemplate x:Key="StateTemplate" DataType="{x:Type local:State}" ItemTemplate="{StaticResource CountyTemplate}">
		<HierarchicalDataTemplate.ItemsSource>
			<Binding>
				<Binding.Source>
					<CollectionViewSource Source="{Binding Path=Counties}">
						<CollectionViewSource.SortDescriptions>
							<scm:SortDescription PropertyName="CountyName" />
						</CollectionViewSource.SortDescriptions>
					</CollectionViewSource>
				</Binding.Source>
			</Binding>
		</HierarchicalDataTemplate.ItemsSource>
		
		<TextBlock Text="{Binding StateName}" />
	</HierarchicalDataTemplate>

This approach also fails to show counties and cities because the Binding in the CollectionViewSource's Source property does not have a DataContext. The Binding used as the HierarchicalDataTemplate's ItemsSource doesn't propagate its DataContext to the CollectionViewSource within it.  The output window in Visual Studio shows the same error messages as in Attempt 2.

### Attempt 4

Since CollectionViewSource doesn't seem to be working, the next logical step is to do the sorting ourselves in code, and to place this code in a Converter:

	<HierarchicalDataTemplate x:Key="StateTemplate" DataType="{x:Type local:State}" ItemsSource="{Binding Counties, Converter={StaticResource SortCountiesConverter}}" ItemTemplate="{StaticResource CountyTemplate}">
		<TextBlock Text="{Binding StateName}" />
	</HierarchicalDataTemplate>
	
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

This approach works if your data source doesn't change. It sorts each level of the hierarchy when the application starts, but if you add items to the hierarchy later, those changes are not reflected in the UI. This is because the code uses Linq to do the sorting and returns a new (sorted) collection. The code doesn't include any mechanism to re-sort the collection and propagate changes to the UI when new items are added to the original collection.

### Solution

This brings us to the final approach, and to a working solution. Instead of using Linq to return a new collection, we can simply return a sorted WPF view that points to the original collection:

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

Since we return a CollectionView that wraps the original collection, any changes in the collection are reflected in the UI. 

Although we couldn't solve the problem entirely in XAML, the code we ended up with is very simple and achieves the desired result while taking source changes into account. 

## Silverlight

In Silverlight, elements added to the resources of a parent element don't inherit the DataContext of that parent element. For this reason, it doesn't make sense to consider attempt 1 described above as a possible approach for Silverlight --none of the CollectionViewSources would have a DataContext and you would get an AG_E_PARSER_BAD_PROPERTY_VALUE runtime error.

Attempt 2 also doesn't apply to Silverlight because templates do not have a Resources section in Silverlight. Again, you would get an AG_E_PARSER_BAD_PROPERTY_VALUE runtime error.

Attempt 3 makes sense to consider in Silverlight, but it won't work for the same reasons as WPF. Except in Silverlight, instead of getting a binding error in the output window, you would see a AG_E_PARSER_BAD_PROPERTY_VALUE runtime error, once again.

Attempt 4 also doesn't work for the same reasons as WPF. It sorts the items correctly, but it doesn't take into account any changes to the data source. I've included this approach in the Silverlight project associated with this post.

The solution that I showed for WPF also works for Silverlight with some minor changes. Silverlight's CollectionViewSource doesn't have a GetDefaultView static method, so we need to create a new CollectionViewSource and use its View property to get at the view in the converters. Here's the Silverlight-compatible version of the code I showed above for WPF:

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

Other minor changes had to be made to the Silverlight project. In particular, HierarchicalDataTemplate, TabControl and TreeView had to refer to the Silverlight SDK, since they're not present in the main download. Also, Silverilght 3 doesn't support implicit styles, so I used the toolkit's ImplicitStyleManager to expand all TreeViewItems (you can read more about this technique <a href="http://www.zagstudio.com/blog/490">here</a>). Keep in mind that Silverlight 4 supports implicit styles natively, so pretty soon there will be no need to use ImplicitStyleManager any more.

