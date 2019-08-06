# Silverlight's CollectionViewSource


In my <a href="..\59-WPFCollectionViewSource">last post</a>, I explained the reasoning behind adding CollectionViewSource to WPF. In this post I will talk about CollectionViewSource in Silverlight (introduced in Silverlight 3) and compare it with its WPF counterpart.

If you recall from my last post, CollectionViewSource was added to WPF mainly to permit view-related scenarios to be expressed in XAML (so they could be tooled), and also to aid in remembering the current item of previously displayed collections. The reasons for adding CollectionViewSource to Silverlight were very different. Before the introduction of CollectionViewSource, Silverlight collections were not automatically wrapped by views - in fact the concept of "views" didn't even exist in Silverlight 2. There was no way to sort or filter a collection (other than modifying the collection itself), or to build a master-detail scenario based on currency (although you could create a master-detail scenario based on selection). The introduction of CollectionViewSource enabled all of those scenarios in Silverlight, while improving compatibility with WPF.

Just like in WPF, a class is considered a "view" if it implements ICollectionView. All the views that we're used to interacting with derive from CollectionView, which in turn implements ICollectionView. Silverlight provides implementations only for EnumerableCollectionView and ListCollectionView, which means that it is able to generate views only for collections that implement IEnumerable or IList. These are by far the two most common scenarios. Unlike WPF, Silverlight's CollectionView, EnumerableCollectionView and ListCollectionView classes are all internal. 

In addition to these, Silverlight also contains a PagedCollectionView class (this is unique to Silverlight). You can manually wrap your collection with this view to add filtering, sorting, grouping, currency tracking and paging to your collection. <a href="http://timheuer.com/blog/archive/2009/11/04/updated-silverlight-3-datagrid-grouping-data-pagedcollectionview.aspx">Tim Heuer</a> shows an example of its usage. Silverlight's CollectionViewSource, on the other hand, provides the ability to filter, sort, and track currency, but it does not offer the ability to group data.

Unlike WPF, Silverlight only wraps collections with a view when CollectionViewSource is used as an intermediary. If you simply bind an ItemsControl to a collection, no "default view" will be created internally for you. Also, since the ItemCollection class doesn't implement ICollectionView, it's not possible to sort or filter non-data bound items that have been added to an ItemsControl.

Just like in WPF, Silverlight's CollectionViewSource creates a view to wrap a collection when its Source property points to a new collection. In both platforms, it is not necessary to specify "Path=View" when binding to the CollectionViewSource - the binding does that automatically. Here's the syntax you use to bind to a CollectionViewSource:

	<UserControl.Resources>
		<CollectionViewSource x:Key="cvs" />
	</UserControl.Resources>
	
	<ListBox ItemsSource="{Binding Source={StaticResource cvs}}" />

Notice that I didn’t have to specify "{Binding Source={StaticResource cvs}, Path=View}" to bind to the view exposed by the CollectionViewSource. Both syntaxes are equivalent, but the second is unnecessary - the binding engine knows to drill into the View property when given a CollectionViewSource.

In WPF, when the CollectionViewSource points to several collections throughout its life, it creates a view for each of them and remembers those views. My last post explains how this feature enables a common scenario that would be a bit of work to implement without CollectionViewSource. I'm very glad to say that this feature has also been implemented in Silverlight, so the selection behavior of my previous post's WPF sample works the same way in Silverlight.

In fact, porting that sample to Silverlight was straightforward. The only difference was that in WPF, Bindings within the resources inherit the Window's DataContext. This enabled me to write the following code/XAML to bind the CollectionViewSource's Source to the Window's DataContext:

	this.DataContext = new Mountains();
	
	<CollectionViewSource Source="{Binding}" x:Key="cvs1"/>

This same feature is not present in Silverlight 3. Here's one equivalent way of implementing that behavior in Silverlight:

	<local:Mountains x:Key="mountains" />
	<CollectionViewSource Source="{Binding Source={StaticResource mountains}}" x:Key="cvs1"/>

