# Data virtualization

My <a href="..\51-UIVirtualization">last post</a> covered the current UI virtualization support on Silverlight and WPF. In this post, I will discuss another technique that helps improve performance when dealing with large data sets: data virtualization. 

A control that uses data virtualization knows to keep in memory no more data items than the ones needed for display. For example, if a data-virtualized ListBox is data bound to 1,000,000 data items and only displays 100 at a single point in time, it only keeps about 100 data items in memory. If the user scrolls, the data-virtualized ListBox knows to swap the items in memory in a way that optimizes performance and memory consumption.

I talked about a possible solution that provides partial data virtualization for TreeView in a <a href="..\40-TreeViewPerformancePart2">previous post</a>. I discussed that solution in the context of WPF, but it could just as easily be used for Silverlight. The discussion in this post addresses data virtualization for non-hierarchical scenarios.

## Data virtualization in WPF

WPF has no built-in generic support for data virtualization. 

Fortunately, <a href="http://www.codeproject.com/KB/WPF/WpfDataVirtualization.aspx">Paul McClean</a> and <a href="DataVirtualization.pdf">Vincent Van Den Berghe</a> have recently come up with approaches to work around this limitation. These two solutions haven't been advertised enough, in my opinion! They're both great solutions and easy to adapt to your specific scenario.

Paul and Vincent's solutions are very similar to each other - they both implement a layer that is capable of managing data items that are kept in memory. This layer knows when to fetch more data items and when to let old data items be garbage collected, based on the user's scrolling patterns. 

Both solutions take advantage of the fact that when a WPF ListBox is data bound to an IList, it only accesses the list's Count property and the data items it needs to display on the screen. Both Paul and Vincent implemented a "fake" collection that knows the overall count of the real collection and knows how to retrieve a particular data item, even if that data item is not in memory. This collection essentially manages a cache of items and, when asked for new data items, it's capable of retrieving a new set of data items from the database, discarding data items that are no longer in use.

The implementation details are quite different, though (this is expected - neither of them was aware of the other one's work).

Paul implemented his own collection called VirtualizingCollection&lt;T&gt;. This collection has a handle to an IItemsProvider, which is a class that knows how to fetch the overall item count and a consecutive list of items from the database (or web service, XML file or whatever data store you're using). VirtualizingCollection&lt;T&gt; keeps the list of non-virtualized data items and decides when new items should be retrieved. Its indexer immediately returns a data item if it's already in memory, or loads more data from the database if it's not. Paul also implemented an extension to this collection called AsyncVirtualizingCollection&lt;T&gt; that provides the ability to do expensive query operations without blocking the UI thread.

Vincent implemented not only his own collection, but also a custom view type for that collection. His collection (VirtualList&lt;T&gt;) implements ICollectionViewFactory, which allows the collection to say that it wants WPF to create a VirtualListCollectionView&lt;T&gt; when binding to it. His view has a handle to the collection and knows to forward any data loading operations to the collection, as well as any sorting and filtering the user may have specified. The data item caching code is in VirtualListBase&lt;T&gt;, which is the base class for both the view and the collection. Vincent also implements a DataRefBase&lt;T&gt; class that wraps each data item, exposing all of its properties using the ICustomTypeDescriptor interface, and adding property change notifications if not present in the original data type.

At the end of this post, I link to an xbap that shows both solutions running side-by-side. Below I compare different aspects of these solutions, and discuss pros and cons of each solution when applicable.

### Using these solutions in your project

If you want to use Paul's solution, you need to provide your own implementation of IItemsProvider. This is where you would add code that retrieves the overall count and a page of items from your database. Then you just need to bind your control to a collection of type AsyncVirtualizingCollection&lt;T&gt;. Here is the example included in Paul's sample code:

	public class DemoCustomerProvider : IItemsProvider<PaulsCustomer>
	{
		private readonly int _count;
		private readonly int _fetchDelay;
	
		public DemoCustomerProvider(int count, int fetchDelay)
		{
			_count = count;
			_fetchDelay = fetchDelay;
		}
	
		public int FetchCount()
		{
			Thread.Sleep(_fetchDelay);
			return _count;
		}
	
		public IList<PaulsCustomer> FetchRange(int startIndex, int count)
		{
			Thread.Sleep(_fetchDelay);
		
			List<PaulsCustomer> list = new List<PaulsCustomer>();
			for (int i = startIndex; i < startIndex + count; i++)
			{
				PaulsCustomer customer = new PaulsCustomer { Id = i, Name = "Customer " + i };
				list.Add(customer);
			}
			return list;
		}
	}
	
	DemoCustomerProvider customerProvider = new DemoCustomerProvider(numItems, fetchDelay);
	sample1.DataContext = new AsyncVirtualizingCollection<PaulsCustomer>(customerProvider, pageSize, pageTimeout);

In Vincent's solution, you need to define a "Load" function somewhere in your ViewModel and pass that as a parameter to the collection. For example:

	private int Load(SortDescriptionCollection sortDescriptions, Predicate<object> filter, VincentsCustomer[] customers, int startIndex)
	{
		Thread.Sleep(fetchDelay);
	
		// Sorting
		bool isDescending = sortDescriptions != null && sortDescriptions.Count > 0 && sortDescriptions[0].Direction == ListSortDirection.Descending;
		int customerIndex;
	
		for (int i = 0; startIndex < numItems && i < customers.Length; ++i, ++startIndex)
		{
			customerIndex = isDescending ? numItems - startIndex - 1 : startIndex;
			customers[i] = new VincentsCustomer { Id = customerIndex, Name = "Customer " + customerIndex };
		}
	
		return numItems;
	}
	
	sample2.DataContext = new VirtualList<VincentsCustomer>(Load, 6 /* # of pages in memory at the same time */, pageSize);

### Caching

Paul's solution divides the original collection in pre-defined pages. When an item is accessed, Paul's solution brings into memory the page that includes it, as well as the previous or next page, depending on whether the item belongs to the first or second half of its page. Paul also keeps track of the time each page is accessed. Every time a new item is retrieved, pages that haven't been touched for a certain amount of time are discarded.

Vincent keeps a linked list of pages in memory. When a new page is brought into memory or an existing page is accessed, it is moved to the beginning of the list. If the number of pages exceeds a specified limit, the pages at the end of the list are discarded.

### Sorting and filtering

Paul's solution doesn't address sorting and filtering. One way to add this functionality to his code is to change the FetchRange method of IItemsProvider to accept sorting and filtering information. In your implementation of IItemsProvider, you would implement FetchRange taking that information into account.

Because Vincent implements his own view, you can add SortDescriptions and a filter delegate as usual. Vincent takes care of passing this information to the Load method. It's up to you to create a database query that incorporates that information in your implementation of Load.

### Threading

Paul's solution fetches the data in a second thread, therefore it doesn't block the UI. The code that enables this scenario is in AsyncVirtualizingCollection&lt;T&gt;.

In Vincent's solution, everything happens synchronously. This means that the UI will be unresponsive while the database is being queried.

### DataTemplates

In Paul's solution, we can use implicit DataTemplates as usual (a DataTemplate is "implicit" when it specifies a DataType but no key).

In Vincent's solution, the items in the collection are of type DataRefBase&lt;T&gt;, which wraps your original type and provides property change notifications. Unfortunately, you can't use an implicit template in this scenario because the type is generic. So, in this post's project, I opted to use the DataTemplate explicitly when using Vincent's solution (I refer to the DataTemplate using an explicit key).

(XAML 2009 adds support for generic types, but this feature is not yet available for compiled XAML scenarios. You can read more about this in <a href="http://blogs.windowsclient.net/rob_relyea/archive/2009/06/01/xaml-using-generic-types-in-xaml-2009.aspx">Rob Relyea's blog</a>.)

!!Master-detail Scenario

Vincent's solution works well when I implement a master-detail scenario by synchronizing the current and selected items:

	<ListView ItemsSource="{Binding}" IsSynchronizedWithCurrentItem="True"  ...>
	<ContentControl Content="{Binding Path=/}" ... />

Paul's solution doesn't work with the XAML above because WPF's ListCollectionView calls IndexOf to track the current item, and Paul's collection doesn't support IndexOf. As an alternative, I implemented the master-detail scenario by binding the ContentControl's Content to the ListView's SelectedItem:

	<ListView ItemsSource="{Binding}" Name="lv1" ...>
	<ContentControl Content="{Binding ElementName=lv1, Path=SelectedItem}" ... />

This second master-detail implementation works just as well as the first, and in fact, many people find the second solution easier to understand.

### Selection

One shortcoming of Paul's solution is the fact that a "collection reset" notification is raised every time a new page is loaded. As a result, selection is lost every time a new page is loaded. For example,  if a user holds the down-arrow key to scroll quickly through a list, selection will jump back to the top of the list when a new page of data items is loaded. 

This problem might be addressed by providing more granular collection change notifications when new data is available (look at the FireCollectionReset method in AsyncVirtualizingCollection&lt;T&gt;).

### Summary

When comparing Paul's solution to Vincent's, you may prefer one over the other, depending which features you find valuable. An ideal solution would combine the best parts of both. Let me know if you come up with a good hybrid!

## Data virtualization in Silverlight

Like WPF, Silverlight does not have built-in support for data virtualization based on scrolling. Unlike WPF, Silverlight has support for data virtualization through paging using PagedCollectionView. I won't talk about PagedCollectionView here - instead I'll focus the discussion on the scrolling based solution.

Vincent's solution requires several features that are not present in the subset of .NET supported by Silverlight, but Paul's solution compiles in Silverlight with very minor changes. However, the implementation of views in Silverlight is less optimized than in WPF, preventing this approach to data virtualization from working. 

The internal implementation of views in WPF accesses only the items that it needs for display (as well as the count of the collection). This is what makes custom data virtualization solutions conceptually easy - we only need to keep in memory the data items that are accessed, and can discard all others.

The implementation of views in Silverlight, on the other hand, accesses all data items in the original collection at load time, independent of how many items it displays on the screen. This affects performance at load time in all scenarios, but because the delay is proportional to the count of the collection, it especially affects scenarios with large data sets. There is no way around this, other than re-implementing large portions of Silverlight's collection views code. If you use .NET Reflector, you can look at this code in the InitializeSnapshot method of EnumerableCollectionView. This is, in my opinion, a big limitation of Silverlight that I am hoping will be addressed soon.

This fact increases the load time significantly, but by itself it shouldn't cause the UI to hang for too long  (depending on the number of items in the collection) because Paul's solution loads pages asynchronously. However, because the collection is reset every time a new page is brought into memory, the effects of looping through all items are magnified. For example, if your collection has 100,000 items and the page size is 100 items, InitializeSnapshot's loop will bring 1,000 pages into memory at load time, asynchronously. As each of those pages is ready to be loaded, a collection reset is triggered, causing Silverlight to loop through all 100,000 items again, causing all pages to be loaded into memory again. At the end of this post, I provide a link to a Silverlight project with Paul's data virtualization implementation so you can see it hang and debug it in case you’re curious. 

I am not aware of any custom data virtualization solution based on scrolling that works with Silverlight at this point, so here's a fun challenge for you! Please send me email if you come up with a solution for this problem.



*Update: If you're looking for a data virtualization solution for WPF, make sure you also read <a href="..\57-DataVirtualization">my more recent post</a>.

