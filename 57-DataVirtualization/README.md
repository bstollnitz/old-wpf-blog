# Improving on existing WPF data virtualization solutions

In a <a href="..\52-DataVirtualization">previous post</a>, I compared two data virtualization techniques implemented by Paul McClean and Vincent Van Den Berghe for WPF. In this post, I describe a solution that combines some of the best features of both. I started with Paul's solution, eliminated a few limitations, and incorporated some of Vincent's ideas.

## Selection

In Paul's solution, a "collection reset" event is used to notify the UI each time a new page is loaded from the database. As a side effect, this notification unintentionally causes a ListBox to lose track of the selected item. This makes it impossible for a user to scroll through a long list using the down-arrow key; every time a new page is loaded, the ListBox selection jumps back to the beginning of the list. The troublesome code can be found in the following methods of AsyncVirtualizingCollection:

	private void LoadPageCompleted(object args)
	{
		int pageIndex = (int)((object[]) args)[0];
		IList<T> page = (IList<T>)((object[])args)[1];
	
		PopulatePage(pageIndex, page);
		IsLoading = false;
		FireCollectionReset();
	}
	
	private void FireCollectionReset()
	{
		NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
		OnCollectionChanged(e);
	}

One possible solution to this problem is to provide more fine-grained add and remove notifications for the new items, instead of a collection reset. Implementing this is not as straightforward as it seems, though, because of the combination of the following two behaviors : 1) When WPF receives a collection change notification for a newly added item, ListCollectionView accesses that item using the collection's indexer, even if the item is not visible in the UI. 2) When an item is accessed, Paul's caching heuristics load its page into memory, as well as the previous or next page depending on whether the item belongs to the first of second half of its page.

With this information, you can probably guess what happens when we provide fine-grained collection notifications. When a page is loaded, we notify WPF that a few items were added to the collection, ListCollectionView accesses each one of those items one by one, triggering a load of the subsequent page, which in turn notifies WPF that a more items were added to the collection, which causes the ListCollectionView to access each one, and so on. Eventually, the whole collection gets loaded, which is exactly what we're trying to avoid.

We could use fine-grained notifications with either of two possible approaches: 1) change the caching heuristics so that neighboring pages are no longer loaded; or 2) implement our own view (as a replacement for ListCollectionView) that doesn't call the collection indexer to access each newly added item. Either approach would fix the problem, but they would not fix another related problem. If we happen to select an item that is not yet loaded, selection would be lost when the item finishes loading. This would happen because selection is tracked based on the actual data item - not its index within the ListBox. If I press the down-arrow key until I select an item that hasn't yet been loaded, when its data item changes at load time (from null to the actual data), the ListBox's selected item is no longer referring to that same item.

This train of thought made it clear that Vincent's technique of wrapping each data item could solve all these selection issues. When using data wrappers, the data items associated with each ListBoxItem don't ever change - they're the wrappers themselves. The data wrappers are not replaced when data loads, and therefore WPF doesn't lose track of the selected item. What changes is the data within the wrapper, which means we can now raise property change notifications to update the UI, instead of collection change notifications. This is good news, since property change notifications are very fine-grained, and they work across threads.

My data wrapper class is called DataWrapper<T> and among other properties it contains a reference to the actual data. 

	public class DataWrapper<T> : INotifyPropertyChanged where T : class
	{
		private T data;
		...
		public T Data
		{
			get { return this.data; }
			internal set
			{
				this.data = value;
				this.OnPropertyChanged("Data");
				...
			}
		}
		...
	}

Adding wrappers required some changes in the collection code base. In Paul's code, requesting a page would add a new entry in the page dictionary with value null, and populating a page would set that value to the actual page:

	protected virtual void RequestPage(int pageIndex)
	{
		if (!_pages.ContainsKey(pageIndex))
		{
			_pages.Add(pageIndex, null);
			_pageTouchTimes.Add(pageIndex, DateTime.Now);
			LoadPage(pageIndex);
		}
		else
		{
			_pageTouchTimes[pageIndex] = DateTime.Now;
		}
	}
	
	protected virtual void PopulatePage(int pageIndex, IList<T> page)
	{
		if ( _pages.ContainsKey(pageIndex) )
			_pages[pageIndex] = page;
	}

To support data wrappers, I changed the code so that a request for a new page results in the immediate creation of a page full of empty data wrappers . This page is added to the dictionary right away. Later, when the actual data gets loaded, populating the page just fills in the data part of the wrappers.

	protected virtual void RequestPage(int pageIndex)
	{
		if (!_pages.ContainsKey(pageIndex))
		{
			int pageLength = Math.Min(this.PageSize, this.Count - pageIndex * this.PageSize);
			DataPage<T> page = new DataPage<T>(pageIndex * this.PageSize, pageLength);                
			_pages.Add(pageIndex, page);
			LoadPage(pageIndex, pageLength);
		}
		else
		{
			_pages[pageIndex].TouchTime = DateTime.Now;
		}
	}
	
	protected virtual void PopulatePage(int pageIndex, IList<T> dataItems)
	{
		DataPage<T> page;
		if (_pages.TryGetValue(pageIndex, out page))
		{
			page.Populate(dataItems);
		}
	}

## Contains and IndexOf

In Paul's data virtualization solution, VirtualizingCollection does not include an implementation for the Contains and IndexOf methods:

	public bool Contains(T item)
	{
		return false;
	}
	
	public int IndexOf(T item)
	{
		return -1;
	}

As a result, the CurrentItem property of WPF's collection view doesn't track the current item correctly, and therefore we can't implement the Master-Detail scenario by simply binding both a ListBox and a ContentControl to the collection. There are other scenarios equally affected by this.

Providing an implementation for these methods was relatively straightforward:

	public bool Contains(DataWrapper<T> item)
	{
		foreach (DataPage<T> page in _pages.Values)
		{
			if (page.Items.Contains(item))
			{
				return true;
			}
		}
		return false;
	}
	
	public int IndexOf(DataWrapper<T> item)
	{
		foreach (KeyValuePair<int, DataPage<T>> keyValuePair in _pages)
		{
			int indexWithinPage = keyValuePair.Value.Items.IndexOf(item);
			if (indexWithinPage != -1)
			{
				return PageSize * keyValuePair.Key + indexWithinPage;
			}
		}
		return -1;
	}

## Currency

Providing an implementation for Contains and IndexOf enabled currency (CurrentItem), but there were still some corner cases that didn't work correctly. For example, if I selected an item and then scrolled it off-screen, WPF knew not to virtualize the UI element for that item, but the data was still being virtualized. This also caused problems with currency. 

I needed a way to prevent an item from virtualizing its data if its UI was still available. Adding data wrappers had the fortunate side effect of making the fix for this problem easier. I know that a data wrapper is being used if someone is listening to its property change event. So I was able to add an IsInUse property to the data wrapper with the following implementation:

	public class DataWrapper<T> : INotifyPropertyChanged where T : class
	{
		...
		public event PropertyChangedEventHandler PropertyChanged;
		public bool IsInUse
		{
			get { return this.PropertyChanged != null; }
		}
	}

Similarly, I added a property that determines whether a page has at least one item in use:

	public class DataPage<T> where T : class
	{
		...
		public bool IsInUse
		{
			get { return this.Items.Any(wrapper => wrapper.IsInUse); }
		}
	}

Then I used that property to avoid cleaning up pages that are still in use, within VirtualizingCollection:

	public void CleanUpPages()
	{
		int[] keys = _pages.Keys.ToArray();
		foreach (int key in keys)
		{
			// page 0 is a special case, since WPF ItemsControl access the first item frequently
			if (key != 0 && (DateTime.Now - _pages[key].TouchTime).TotalMilliseconds > PageTimeout)
			{
				bool removePage = true;
				DataPage<T> page;
				if (_pages.TryGetValue(key, out page))
				{
					removePage = !page.IsInUse;
				}
	
				if (removePage)
				{
					_pages.Remove(key);
				}
			}
		}
	}

## IsInitializing + IsLoading

Paul's AsyncVirtualizingCollection has an "IsLoading" property that is set to true when the collection is either counting its items or fetching a page. This is useful so that we can provide visual feedback when we're querying data from the database. On the other hand, it's a bit limiting to have only one property indicating that work is in progress. We don't want to prevent the user from interacting with other items in the ListBox just because scrolling causes a few items to start downloading. Ideally, we would get more fine-grained status information.

To solve this problem, I added an "IsInitializing" property that is true when we're fetching the count, and changed "IsLoading" slightly to inform us when the collection is fetching a new page. The "IsInitializing" property is defined at the collection level, and the "IsLoading" property is defined in the data wrapper.

When the collection count is being fetched (that is, when IsInitializing is true), I display a message in the middle of the empty ListBox and switch to the "Wait" cursor, making it obvious that it's not yet ready for user interaction:

	<ControlTemplate TargetType="{x:Type ListView}">
		<Grid>
			<theme:ListBoxChrome Name="Bd" ... >
				...
			</theme:ListBoxChrome>
			<Grid Background="White" Opacity="0.5" Name="InitializingGrid" Visibility="Collapsed">
				<TextBlock Text="Initializing..." HorizontalAlignment="Center" VerticalAlignment="Center"/>
			</Grid>
		</Grid>
		<ControlTemplate.Triggers>
			...
			<DataTrigger Binding="{Binding Path=IsInitializing}" Value="True">
				<Setter Property="Cursor" Value="Wait" TargetName="InitializingGrid"/>
				<Setter Property="Visibility" Value="Visible" TargetName="InitializingGrid"/>
			</DataTrigger>
		</ControlTemplate.Triggers>
	</ControlTemplate>

When an item is being fetched from the database (that is, when IsLoading is true), I display a message and "Wait" cursor just within the corresponding ListViewItem:

	<ControlTemplate TargetType="{x:Type ListViewItem}">
		...
		<Grid>
			...
			<GridViewRowPresenter ...>
			<StackPanel Name="Loading" Orientation="Horizontal" Grid.RowSpan="2" Visibility="Collapsed">
				<TextBlock Text="Loading item " />
				<TextBlock Text="{Binding ItemNumber}" />
				<TextBlock Text="..." />
			</StackPanel>
		</Grid>
		...
		<ControlTemplate.Triggers>
			<DataTrigger Binding="{Binding IsLoading}" Value="True">
				<Setter TargetName="Loading" Property="Visibility" Value="Visible"/>
				<Setter Property="Cursor" Value="Wait" />
				...
			</DataTrigger>
		</ControlTemplate.Triggers>
	</ControlTemplate>

This is the point where I would normally hand the problem over to a visual designer or an interaction designer. Now that we can get fine-grained information about which data items are loading and which are available, a designer could come up with a variety of ways to display this information to the user.

## Still missing...

Paul's solution assumes the collection is read-only, and my code doesn't really fix that limitation. Although my AsyncVirtualizingCollection will notice if its count has changed when fetching a new page of data, it won't notice at any other time. If you're successful at extending this solution to support dynamic collection changes, I'd love to hear from you!


